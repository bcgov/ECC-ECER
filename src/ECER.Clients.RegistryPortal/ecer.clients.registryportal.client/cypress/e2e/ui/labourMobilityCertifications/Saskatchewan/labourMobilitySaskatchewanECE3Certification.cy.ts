import selectors from "../../../../support/selectors";
const provinceName = "Saskatchewan";
const certificationName = "ECE III";

describe("Labour Mobility - ECE III Certification Transfer Application for Saskatchewan", () => {
  it("should sucessfully create a ECE III LM Certification Transfer Application for Saskatchewan", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.transferButton).click();

    /** Transfer Eligibility */
    cy.contains("Check your transfer eligibility").should("be.visible");

    /** Saskatchewan*/
    cy.get(selectors.transferEligibility.provinceDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(provinceName).click();

    /**Out of province Certification Type */
    cy.get(selectors.transferEligibility.certificationTypeDropDownList)
      .should("exist")
      .click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(certificationName).click();

    cy.contains("Work experience").should("be.visible");

    cy.get(selectors.transferEligibility.programConfirmationRadioYes).check({
      force: true,
    });

    cy.contains(
      "You can apply to transfer your certification to ECE Five Year certification with Infant and Toddler Educator (ITE) and Special Needs Educator (SNE) in B.C.",
    ).should("be.visible");

    /** View Requirements Button */
    cy.get(selectors.transferEligibility.viewRequirementsButton).click();

    /**Requirements */
    cy.contains("Transfer to ECE Five Year certification").should("be.visible");
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Certificate Information */

    cy.contains("Certificate information").should("be.visible");
    cy.contains(
      "ECE Five Year and Special Needs Educator (SNE) and Infant and Toddler Educator (ITE)",
    ).should("be.visible");
    cy.contains(provinceName).should("be.visible");
    cy.contains(certificationName).should("be.visible");
    cy.get(selectors.certificateInformation.nameOnCertificateRadio)
      .first()
      .check({ force: true });
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    cy.ECEFiveYearWorkflow(provinceName, certificationName);
  });
});
