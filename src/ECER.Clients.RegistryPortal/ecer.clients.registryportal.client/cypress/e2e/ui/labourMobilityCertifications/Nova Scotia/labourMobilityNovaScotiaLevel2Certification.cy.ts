import selectors from "../../../../support/selectors";
const provinceName = "Nova Scotia";
const certificationName = "Level 2";

describe("Labour Mobility - Level 2 Certification Transfer Application for Nova Scotia", () => {
  it("should sucessfully create a Level 2 LM Certification Transfer Application for Nova Scotia", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.transferButton).click();

    /** Transfer Eligibility */
    cy.contains("Check your transfer eligibility").should("be.visible");

    /** Nova Scotia*/
    cy.get(selectors.transferEligibility.provinceDropDownList).should("exist").click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(provinceName).click();

    /**Out of province Certification Type */
    cy.get(selectors.transferEligibility.certificationTypeDropDownList).should("exist").click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(certificationName).click();

    cy.contains("Work experience").should("be.visible");

    cy.get(selectors.transferEligibility.programConfirmationRadioNo).check({ force: true });

    cy.contains("You can apply to transfer your certification to ECE One Year certification in B.C.").should("be.visible");

    /** View Requirements Button */
    cy.get(selectors.transferEligibility.viewRequirementsButton).click();

    /**Requirements */
    cy.contains("Transfer to ECE One Year certification").should("be.visible");
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Certificate Information */

    cy.contains("Certificate information").should("be.visible");
    cy.contains("ECE One Year").should("be.visible");
    cy.contains(provinceName).should("be.visible");
    cy.contains(certificationName).should("be.visible");
    cy.get(selectors.certificateInformation.nameOnCertificateRadio).first().check({ force: true });
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    cy.ECEOneYearWorkflow(provinceName, certificationName);
  });
});
