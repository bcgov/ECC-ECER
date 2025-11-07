describe("Renew Expired (for more than 5 Years) ECE One Year Certificate Application", () => {
  it("should create a sucessfull Renewal - ECE One Year Application", () => {
    cy.seedRenewalApplication("ECEOneYear", false, true);

    cy.reload();
    cy.reload();    
    /** Dashboard */
    cy.contains("You cannot renew your ECE One Year certification because it's been expired for over 5 years.").should("be.visible");
  });
});
