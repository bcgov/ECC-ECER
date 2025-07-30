import selectors from "../../../../support/selectors";
const provinceName = "Manitoba";
const certificationName = "ECE 2";

describe("Labour Mobility - ECE 2 Certification Transfer Application for Manitoba", () => {
  it("should sucessfully create a ECE 2 LM Certification Transfer Application for Manitoba", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.transferButton).click();

    /** Transfer Eligibility */
    cy.contains("Check your transfer eligibility").should("be.visible");

    /** Manitoba*/
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
