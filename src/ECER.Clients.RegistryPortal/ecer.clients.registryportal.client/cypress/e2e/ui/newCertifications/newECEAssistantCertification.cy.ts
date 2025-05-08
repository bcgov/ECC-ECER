import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay } from "../../../support/utils";

describe("New ECE Assistant Certificate Application", () => {
  it("should sucessfully create a New ECE Assistant Application", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.applyNowButton).click();

    /** Certification Type */
    cy.get(selectors.certificationType.eceAssistantRadio).check();
    cy.get(selectors.certificationType.continueButton).click();

    /** Application Requirements */
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Contact Information */
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Education */
    cy.get(selectors.education.addEducationButton).click();

    cy.get(selectors.education.transcriptStatusRadioDiv).within(() => {
      cy.get(selectors.elementType.radio).first().check({ force: true });
    });
    cy.get(selectors.education.programNameInput).type("TEST ECE Assistant Course", { force: true });

    /* Start Date - DatePicker*/
    cy.get(selectors.education.programStartDateInput).click({ force: true });
    cy.get(selectors.education.programStartDateInput).type(`${courseStartDay} {enter}`, { force: true });

    /* End Date - DatePicker*/
    cy.get(selectors.education.programEndDateInput).click({ force: true });
    cy.get(selectors.education.programEndDateInput).type(`${courseEndDay} {enter}`, { force: true });

    cy.get(selectors.education.provinceDropDownList).should("exist").type("British Columbia", { force: true });

    cy.get(selectors.education.postSecondaryInstitutionDropDownList).should("exist").type("Other", { force: true });
    cy.get("body").click({ force: true });

    cy.get(selectors.education.institutionNameInput).type("TEST Educational Institution", { force: true });
    cy.get(selectors.education.studentIDInput).type("1234", { force: true });
    cy.get(selectors.education.nameOnTranscriptRadioDiv).within(() => {
      cy.get(selectors.elementType.radio).first().check({ force: true });
    });
    cy.get(selectors.education.saveEducationButton).click();

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Character Reference */
    cy.get(selectors.characterReference.lastNameInput).type("Reference Last Name");
    cy.get(selectors.characterReference.firstNameInput).type("Reference First Name");
    cy.get(selectors.characterReference.emailInput).type("Character_Reference@test.gov.bc.ca");
    cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Application Review and Submit */
    cy.document().its("readyState").should("eq", "complete");
    cy.contains("Review and submit").should("be.visible");
    cy.get(selectors.applicationPreview.certificationType).should("be.visible").should("contain.text", "ECE Assistant");
    cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "Reference First Name");
    cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "Reference Last Name");
    cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");

    cy.get(selectors.applicationPreview.educationCountry).should("be.visible").should("contain.text", "Canada");

    cy.get(selectors.applicationPreview.educationProvince).should("be.visible").should("contain.text", "British Columbia");
    cy.get(selectors.applicationWizard.submitApplicationButton).click();

    /** Application Submitted */
    cy.document().its("readyState").should("eq", "complete");
    cy.get(selectors.applicationSubmitted.pageTitle).should("be.visible").should("contain.text", "Application Submitted");
    cy.get(selectors.applicationSubmitted.applicationSummaryButton).should("be.visible").should("contain.text", "Go to application summary");
  });
});
