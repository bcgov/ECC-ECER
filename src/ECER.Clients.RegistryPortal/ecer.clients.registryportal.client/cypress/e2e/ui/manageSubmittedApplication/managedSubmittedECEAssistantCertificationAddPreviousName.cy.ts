import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay, todayDay } from "../../../support/utils";

describe("Managed Submitted ECE Assistant Certification Add PreviousName", () => {
  it("should sucessfully create a New ECE Assistant Application and add previous name", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.applyNowButton).click();

    /** Certification Type */
    cy.get(selectors.certificationType.applyNowEceAssistantButton).click();

    /** Application Requirements */
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Contact Information */
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Education */
    cy.get(selectors.education.addEducationButton).click();

    cy.get(selectors.education.provinceDropDownList).should("exist").type("British Columbia");

    cy.get(selectors.education.postSecondaryInstitutionDropDownList).should("exist").type("Other");
    cy.get("body").click({ force: true });

    cy.get(selectors.education.institutionNameInput).type("TEST Educational Institution");

    cy.get(selectors.education.programNameInput).type("TEST ECE Assistant Course");

    /* Start Date - DatePicker*/
    cy.get(selectors.education.programStartDateInput).click({ force: true });
    cy.get(selectors.education.programStartDateInput).clear();
    cy.get(selectors.education.programStartDateInput).type(`${courseStartDay} {enter}`);

    /* End Date - DatePicker*/
    cy.get(selectors.education.programEndDateInput).click({ force: true });
    cy.get(selectors.education.programEndDateInput).clear();
    cy.get(selectors.education.programEndDateInput).type(`${courseEndDay} {enter}`);

    cy.get(selectors.education.studentIDInput).type("1234");
    cy.get(selectors.education.nameOnTranscriptRadioDiv).within(() => {
      cy.get(selectors.elementType.radio).first().check({ force: true });
    });

    cy.get(selectors.education.transcriptStatusRadioDiv).within(() => {
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
     // Manage Submitted Application 
     cy.get(selectors.elementType.span_button).contains('Home').click();
     cy.reload();
     cy.get(selectors.elementType.span_button).contains('Manage Application').click();
     cy.get(selectors.elementType.text_links).contains('Proof of previous name').click()
     cy.contains('a','Add previous name').click({ force: true });
     const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
     cy.get('[aria-label="First name"]').type(`First Name_${timestamp}`); 
     cy.get('[aria-label="Middle names (optional)"]').type(`Middle tName_${timestamp}`);
     cy.get('[aria-label="Last name"]').type(`Last Name_${timestamp}`);

     cy.get(selectors.elementType.span_button).contains('Save and continue').click();
     cy.get(selectors.elementType.span_button).contains('Add file').click();
     // Wait for hidden file input to appear in the DOM
     cy.get('input[type="file"]', { timeout: 10000 }).should('exist');

     // Path relative to cypress/fixtures
     const filePath = 'Sample.pdf';

     // Attach file directly to hidden input
     cy.get('input[type="file"]').attachFile(filePath);

     // (Optional) Assert successful upload
     cy.contains('Upload complete').should('be.visible');
     cy.get(selectors.elementType.span_button).contains('Send').click();
  });
});
