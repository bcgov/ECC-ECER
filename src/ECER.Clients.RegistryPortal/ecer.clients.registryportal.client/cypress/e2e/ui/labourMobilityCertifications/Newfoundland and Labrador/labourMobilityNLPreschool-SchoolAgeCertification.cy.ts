import selectors from "../../../../support/selectors";
const provinceName = "Newfoundland and Labrador";
const certificationName = "Preschool (5 Modules) or School Age (4 Modules) Orientation Course";

describe("Labour Mobility - Preschool or School Age Orientation Course Certification Transfer Application for Newfoundland and Labrador", () => {
  it("should sucessfully create a Preschool or School Age Orientation Course LM Certification Transfer Application for Newfoundland and Labrador", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.transferButton).click();

    /** Transfer Eligibility */
    cy.contains("Check your transfer eligibility").should("be.visible");

    /** Alberta*/
    cy.get(selectors.transferEligibility.provinceDropDownList).should("exist").click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(provinceName).click();

    /**Out of province Certification Type */
    cy.get(selectors.transferEligibility.certificationTypeDropDownList).should("exist").click({ force: true });
    cy.get(selectors.elementType.vListItem).contains(certificationName).click();

    cy.contains("You can apply to transfer your certification to ECE Assistant certification in B.C.").should("be.visible");

    /** View Requirements Button */
    cy.get(selectors.transferEligibility.viewRequirementsButton).click();

    /**Requirements */
    cy.contains("Transfer to ECE Assistant certification").should("be.visible");
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Certificate Information */

    cy.contains("Certificate information").should("be.visible");
    cy.contains("ECE Assistant").should("be.visible");
    cy.contains(provinceName).should("be.visible");
    cy.contains(certificationName).should("be.visible");
    cy.get(selectors.certificateInformation.nameOnCertificateRadio).first().check({ force: true });
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    cy.ECEAssistantWorkflow(provinceName, certificationName);
  });
});
