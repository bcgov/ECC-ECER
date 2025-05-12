import "./commands";

// eslint-disable-next-line mocha/no-top-level-hooks
before(() => {
  // Set the viewport (pull width/height from Cypress.env)
  const width = Cypress.env("DEVICE_VIEWPORT")?.WIDTH ?? Cypress.config("viewportWidth");
  const height = Cypress.env("DEVICE_VIEWPORT")?.HEIGHT ?? Cypress.config("viewportHeight");

  cy.viewport(width, height);

  // Visit login and wait for ready
  cy.visit("/login");
  cy.document().its("readyState").should("eq", "complete");

  //reset user state
  cy.resetUserState();

  //login
  cy.login();
});

// eslint-disable-next-line mocha/no-top-level-hooks
after(() => {
  // Logout and Reset cookies & local storage
  cy.logout();
  cy.resetBrowserState();
});
