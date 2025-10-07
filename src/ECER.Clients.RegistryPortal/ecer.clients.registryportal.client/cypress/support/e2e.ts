import "./commands";
import "./labour-mobility-commands";
import 'cypress-file-upload';

// eslint-disable-next-line mocha/no-top-level-hooks
beforeEach(() => {
  // Set the viewport (pull width/height from Cypress.env)
  const width = Cypress.env("DEVICE_VIEWPORT")?.WIDTH ?? Cypress.config("viewportWidth");
  const height = Cypress.env("DEVICE_VIEWPORT")?.HEIGHT ?? Cypress.config("viewportHeight");

  cy.viewport(width, height);

  //Clear saved session if there is a retry
  if (Cypress.currentRetry > 0) {
    cy.log("retry count: " + Cypress.currentRetry);
    cy.log("Resetting browser state in case session state is stale");  
    Cypress.session.clearAllSavedSessions();
  }
 

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
