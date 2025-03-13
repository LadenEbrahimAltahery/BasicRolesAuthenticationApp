using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using RoleBasedBasicAuthentication.Services;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using RoleBasedBasicAuthentication.Interfaces;

namespace RoleBasedBasicAuthentication.Handler
{
    // This class implements a custom Basic Authentication scheme by extending
    // the AuthenticationHandler with AuthenticationSchemeOptions.
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // Stores a reference to the IUserService, which handles user validation and data access.
        private readonly IUserService _userService;

        // Constructor:
        // - Receives necessary dependencies such as AuthenticationSchemeOptions, ILoggerFactory, and UrlEncoder.
        // - Also takes IUserService to validate user credentials.
        // - Passes the first three parameters to the base constructor of AuthenticationHandler.
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IUserService userService)
            : base(options, logger, encoder)
        {
            _userService = userService;
        }

        // Main method that handles the authentication process.
        // The framework calls this method when a request requires authentication.
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header is present in the current HTTP request.
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // If missing, fail the authentication process with a specific error message.
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            // Retrieve the value of the Authorization header.
            var authorizationHeader = Request.Headers["Authorization"].ToString();

            // Attempt to parse the Authorization header into a recognized format (AuthenticationHeaderValue).
            if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                // If it can’t parse the header, fail the authentication.
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            // Ensure that the scheme in the header is "Basic".
            // Basic Authentication has the scheme name "Basic" plus a Base64 encoded string.
            if (!"Basic".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // If the scheme is not Basic, fail.
                return AuthenticateResult.Fail("Invalid Authorization Scheme");
            }

            // Decode the Base64-encoded credentials "<email>:<password>".
            // Split the resulting string on the first ':' to separate email and password.
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter)).Split(':', 2);

            // If we don’t have exactly two parts (email and password), the credentials are invalid.
            if (credentials.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic Authentication Credentials");
            }

            // Extract email and password from the decoded credentials array.
            var email = credentials[0];
            var password = credentials[1];

            try
            {
                // Validate the user using the IUserService’s method.
                // This typically checks if the email and password match a record in the database.
                var user = await _userService.ValidateUserAsync(email, password);

                // If the user is not found (null), fail the authentication with an error message.
                if (user == null)
                {
                    return AuthenticateResult.Fail("Invalid Username or Password");
                }

                // Retrieve the roles associated with this user to build role-based claims.
                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

                // Create a list of Claims that will represent the authenticated user’s identity.
                var claims = new List<Claim>
                {
                    // This claim uniquely identifies the user (using their ID).
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                    // This claim holds the user's email address.
                    new Claim(ClaimTypes.Name, user.Email)
                };

                // Loop through each user role and add a corresponding role claim.
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Create a ClaimsIdentity, specifying the authentication scheme name.
                var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);

                // Create a ClaimsPrincipal that can be checked against policies or role requirements.
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Build the AuthenticationTicket with the principal and scheme name.
                var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                // Indicate a successful authentication by returning AuthenticateResult.Success.
                return AuthenticateResult.Success(authenticationTicket);
            }
            catch
            {
                // If any error occurs during user validation or claims creation,
                // fail the authentication with a general error message.
                return AuthenticateResult.Fail("Error occurred during authentication");
            }
        }

        // This override method is called by the framework when a challenge response is needed.
        // A challenge response indicates the client needs to provide credentials (e.g., 401 Unauthorized).
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Adds the "WWW-Authenticate" header with "Basic" so that clients understand they should
            // retry the request with Basic credentials if unauthorized.
            Response.Headers["WWW-Authenticate"] = "Basic";

            // Call the base method to finalize the challenge response.
            await base.HandleChallengeAsync(properties);
        }
    }
}