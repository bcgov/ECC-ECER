/// <reference types="cypress" />

describe("Final cleanup - logout & reset browser state", () => {
  it("logs out portal user and resets browser state", () => {
    cy.document().its("readyState").should("eq", "complete");
    cy.contains("Your ECE certifications").should("be.visible");
    // calls custom commands
    cy.logout();
    cy.resetBrowserState();
    cy.then(() => {
      Cypress.session.clearAllSavedSessions();
    });
  });
});
