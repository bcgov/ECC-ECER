import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay, todayDay } from "../../../support/utils";
const provinceName = "Alberta";
const certificationName = "Early Childhood Educator Level 1";

describe(" Active ECE One Year Certificate Applicatio plus add labour Mobility", () => {
  it("should create a sucessfull Labour mobility Application dd labour Mobility", () => {
    cy.seedRenewalApplication("ECEOneYear", true, false);

    cy.reload();
    /** Dashboard */
    cy.get(selectors.dashboard.multiApplyButton).eq(1).click();

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
