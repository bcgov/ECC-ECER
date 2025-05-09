describe("BCSC Authentication Tests", () => {
  it("should log in successfully", () => {
    // Check if the profile page is available.
    cy.visit("/profile");
    cy.document().its("readyState").should("eq", "complete");
    cy.contains("Email address").should("be.visible");
  });
});
