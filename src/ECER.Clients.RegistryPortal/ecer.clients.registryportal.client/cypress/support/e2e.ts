import "./commands";

// eslint-disable-next-line mocha/no-top-level-hooks
beforeEach(() => {
  // Reset cookies & local storage
  cy.resetState();

  // Set the viewport (pull width/height from Cypress.env)
  cy.viewport(Cypress.env("DEVICE_VIEWPORT").WIDTH, Cypress.env("DEVICE_VIEWPORT").HEIGHT);

  // Visit login and wait for ready
  cy.visit("/login");
  cy.document().its("readyState").should("eq", "complete");

  // Reset whatever user state you need
  cy.resetUserState();

  //login
  cy.login();
});
