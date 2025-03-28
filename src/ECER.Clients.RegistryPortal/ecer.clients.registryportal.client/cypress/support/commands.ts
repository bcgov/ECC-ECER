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
  const user = username ?? (Cypress.env("PORTAL_USER").BCSC_USERNAME as string);
  const pass = password ?? (Cypress.env("PORTAL_USER").BCSC_PASSWORD as string);

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

Cypress.Commands.add("resetUserState", () => {
  const apiUrl: string = Cypress.env("API_URL");
  const apiKey: string = Cypress.env("API_KEY");
  const externalUserId: string = Cypress.env("PORTAL_USER").EXTERNAL_USER_ID;

  return cy
    .request({
      method: "DELETE",
      url: `${apiUrl}/api/E2ETests/user/reset`,
      headers: {
        "X-API-KEY": apiKey,
        "EXTERNAL-USER-ID": externalUserId,
      },
      failOnStatusCode: false,
    })
    .then((response) => {
      expect(response.status).to.eq(200);
      cy.log("User State Reset sucessfull");
      // Return the response if needed for further chaining
      return cy.wrap(response);
    });
});
