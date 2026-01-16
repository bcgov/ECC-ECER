import selectors from "../../../support/selectors";

describe("Labour Mobility - Ineligible Certification Application", () => {
  it("should sucessfully verify an Ineligible LM Certification Application", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.transferButton).click();

    /** Transfer Eligibility */
    cy.contains("Check your transfer eligibility").should("be.visible");

    /** New Brunswick */
    cy.get(selectors.transferEligibility.provinceDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem).contains("New Brunswick").click();
    cy.contains(
      "The province or territory you hold certification in is not eligible to be transferred to B.C.",
    ).should("be.visible");

    /** Northwest Territories */
    cy.get(selectors.transferEligibility.provinceDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem)
      .contains("Northwest Territories")
      .click();
    cy.contains(
      "The province or territory you hold certification in is not eligible to be transferred to B.C.",
    ).should("be.visible");

    /** Nunavut */
    cy.get(selectors.transferEligibility.provinceDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem).contains("Nunavut").click();
    cy.contains(
      "The province or territory you hold certification in is not eligible to be transferred to B.C.",
    ).should("be.visible");

    /** Quebec */
    cy.get(selectors.transferEligibility.provinceDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem).contains("Quebec").click();
    cy.contains(
      "The province or territory you hold certification in is not eligible to be transferred to B.C.",
    ).should("be.visible");
  });
});
