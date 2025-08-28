import "./commands";
import "./labour-mobility-commands";

// eslint-disable-next-line mocha/no-top-level-hooks
beforeEach(() => {
  // Set the viewport (pull width/height from Cypress.env)
  const width = Cypress.env("DEVICE_VIEWPORT")?.WIDTH ?? Cypress.config("viewportWidth");
  const height = Cypress.env("DEVICE_VIEWPORT")?.HEIGHT ?? Cypress.config("viewportHeight");

  cy.viewport(width, height);



  //reset user state
  cy.resetUserState();
  
// Visit login and wait for ready
cy.visit("/login");
cy.document().its("readyState").should("eq", "complete");
  //login
  // cache under the key "bcsc-user"
  cy.session(
    "bcsc-user",
    () => {
      // this calls login command
      cy.login();
    },
    {
      cacheAcrossSpecs: true, // this will cache the session across all specs
    },
  );
  //load app
  cy.visit("/");
});
