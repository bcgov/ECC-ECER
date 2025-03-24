// Custom command to reset state (clear cookies, local storage, and session storage)
Cypress.Commands.add("resetState", () => {
  cy.clearCookies();
  cy.clearLocalStorage();
  // Clear session storage if your app uses it
  cy.window().then((win) => {
    win.sessionStorage.clear();
  });
});

// Custom command to log in using default or provided credentials.
Cypress.Commands.add("login", (username?: string, password?: string) => {
  const user = username ?? (Cypress.env("BCSC_USERNAME") as string);
  const pass = password ?? (Cypress.env("BCSC_PASSWORD") as string);

  // Adjust the URL if login page is at a different route.
  cy.visit("/login");
  // Perform login steps:
  // 1. Click the button with the text "Log in with BC Services Card".
  cy.contains("button", "Log in with BC Services Card").should("be.visible").click();

  cy.origin("https://idtest.gov.bc.ca", { args: { user, pass } }, ({ user, pass }) => {
    // 2. Click on the tile (or button) with the given id.
    cy.get("#tile_test_with_username_password_device_div_id").click({ force: true });
    // 3. Enter the username and password.
    cy.get('input[name="username"]').type(user, { force: true });
    cy.get('input[name="password"]').type(pass, { force: true });
    // 4. Click the submit button.
    cy.get("#submit-btn").click({ force: true });
  });

  // 5. Verify that the login was successful.
  cy.url().should("not.include", "/login");
  cy.document().its("readyState").should("eq", "complete");
  cy.contains("Your ECE certifications").should("be.visible");
});

Cypress.Commands.add("resetApplicationState", (applicationId: string) => {
  const apiUrl: string = Cypress.env("API_URL");
  const apiKey: string = Cypress.env("API_KEY");

  return cy
    .request({
      method: "DELETE",
      url: `${apiUrl}/api/E2ETests/applications/delete/${applicationId}`,
      headers: {
        "X-API-KEY": apiKey,
      },
      failOnStatusCode: false,
    })
    .then((response) => {
      expect(response.status).to.eq(200);
      cy.log("Application State has been Reset sucessfully");
      cy.log("Navigate to the base URL");
      // Navigate to the base URL
      cy.visit("/");
      // Return the response if needed for further chaining
      return cy.wrap(response);
    });
});

// Global variable to store the latest response message from either API.
let applicationID: string = "";
// Custom command to register intercepts for GET and PUT endpoints.
Cypress.Commands.add("registerApiIntercepts", () => {
  // Reset the variable if necessary at the start of each test.
  applicationID = "";
  // Intercept GET requests to '/api/applications/'
  cy.intercept("GET", "/api/applications/", (req) => {
    req.continue((res) => {
      const body = res.body[0] as { id?: string };
      if (body?.id) {
        applicationID = body.id;
      }
    });
  }).as("getApplicationApiCall");

  // Intercept PUT requests to '/api/draftapplications'
  cy.intercept("PUT", "/api/draftapplications", (req) => {
    req.continue((res) => {
      const body = res.body.application as { id?: string };
      if (body?.id) {
        applicationID = body.id;
      }
    });
  }).as("putDraftApplicationApiCall");

  // Return a cy chainable for consistency.
  return cy.wrap(null);
});

// Optional custom command to retrieve the latest response string.
Cypress.Commands.add("getApplicationID", () => {
  return cy.wrap(applicationID);
});
