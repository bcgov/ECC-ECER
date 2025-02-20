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
  const user = username || (Cypress.env("username") as string);
  const pass = password || (Cypress.env("password") as string);

  // Adjust the URL if login page is at a different route.
  cy.visit("/login");
  // Perform login steps:
  // 1. Click the button with the text "Log in with BC Services Card".
  cy.contains("button", "Log in with BC Services Card", { timeout: 10000 }).should("be.visible").click();

  cy.origin("https://idtest.gov.bc.ca/", { args: { user, pass } }, ({ user, pass }) => {
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
  cy.contains("Your ECE certifications", { timeout: 20000 }).should("be.visible");
});
