import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay, todayDay } from "../../../support/utils";

describe("New ECE 5 Year + ITE & SNE Certificate Application", () => {
  it("should sucessfully create a New ECE 5 Year + ITE & SNE Application", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.applyNowButton).click();

    /** Certification Type */
    cy.get(selectors.certificationType.applyNowEceFiveYearButton).click();

    /** Application Requirements */
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Contact Information */
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Education */
    cy.get(selectors.education.addEducationButton).click();

    cy.get(selectors.education.provinceDropDownList)
      .should("exist")
      .type("British Columbia");

    cy.get(selectors.education.postSecondaryInstitutionDropDownList)
      .should("exist")
      .type("Other");
    cy.get("body").click({ force: true });

    cy.get(selectors.education.institutionNameInput).type(
      "TEST Educational Institution",
    );

    cy.get(selectors.education.programNameInput).type("TEST ECE 5 Year Course");

    /* Start Date - DatePicker*/
    cy.get(selectors.education.programStartDateInput).click({ force: true });
    cy.get(selectors.education.programStartDateInput).clear();
    cy.get(selectors.education.programStartDateInput).type(
      `${courseStartDay} {enter}`,
    );

    /* End Date - DatePicker*/
    cy.get(selectors.education.programEndDateInput).click({ force: true });
    cy.get(selectors.education.programEndDateInput).clear();
    cy.get(selectors.education.programEndDateInput).type(
      `${courseEndDay} {enter}`,
    );

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
    cy.get(selectors.characterReference.lastNameInput).type(
      "CharacterReferenceLastName",
    );
    cy.get(selectors.characterReference.firstNameInput).type(
      "CharacterReferenceFirstName",
    );
    cy.get(selectors.characterReference.emailInput).type(
      "Character_Reference@test.gov.bc.ca",
    );
    cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Work Experience Reference */
    cy.document().its("readyState").should("eq", "complete");
    cy.get(selectors.workExperienceReference.addReferenceButton).click();

    cy.get(selectors.workExperienceReference.lastNameInput).type(
      "WorkReferenceLastName",
    );
    cy.get(selectors.workExperienceReference.firstNameInput).type(
      "WorkReferenceFirstName",
    );
    cy.get(selectors.workExperienceReference.emailInput).type(
      "WorkExperience_Reference@test.gov.bc.ca",
    );
    cy.get(selectors.workExperienceReference.phoneNumberInput).type(
      "1234567890",
    );
    cy.get(selectors.workExperienceReference.hoursInput).type("500");

    cy.get(selectors.workExperienceReference.saveReferenceButton).click();

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Application Review and Submit */
    cy.document().its("readyState").should("eq", "complete");

    cy.contains("Review and submit").should("be.visible");
    cy.get(selectors.applicationPreview.certificationType)
      .should("be.visible")
      .should("contain.text", "ECE Five Year");
    cy.get(selectors.applicationPreview.characterReferenceFirstName)
      .should("be.visible")
      .should("contain.text", "CharacterReferenceFirstName");
    cy.get(selectors.applicationPreview.characterReferenceLastName)
      .should("be.visible")
      .should("contain.text", "CharacterReferenceLastName");
    cy.get(selectors.applicationPreview.characterReferenceEmail)
      .should("be.visible")
      .should("contain.text", "Character_Reference@test.gov.bc.ca");
    cy.get(selectors.applicationPreview.educationCountry)
      .should("be.visible")
      .should("contain.text", "Canada");
    cy.get(selectors.applicationPreview.educationProvince)
      .should("be.visible")
      .should("contain.text", "British Columbia");

    cy.get(selectors.applicationPreview.workReferenceName)
      .should("be.visible")
      .should("contain.text", "WorkReferenceFirstName");
    cy.get(selectors.applicationPreview.workReferenceName)
      .should("be.visible")
      .should("contain.text", "WorkReferenceLastName");
    cy.get(selectors.applicationPreview.workReferenceEmail)
      .should("be.visible")
      .should("contain.text", "WorkExperience_Reference@test.gov.bc.ca");

    cy.get(selectors.applicationWizard.submitApplicationButton).click();

    /** Application Submitted */
    cy.document().its("readyState").should("eq", "complete");
    cy.get(selectors.applicationSubmitted.pageTitle)
      .should("be.visible")
      .should("contain.text", "Application Submitted");
    cy.get(selectors.applicationSubmitted.applicationSummaryButton)
      .should("be.visible")
      .should("contain.text", "Go to application summary");
    // Manage Submitted Application
    cy.contains("span.v-btn__content", "Home").click();
    cy.reload();
    cy.contains("span.v-btn__content", "Manage Application").click();
    cy.contains("p.text-links", "Course outlines or syllabi:").click();
    cy.get(
      'input[aria-label="I have my course outlines or syllabi and will upload them now."]',
    ).check({ force: true });
    cy.contains("span.v-btn__content", "Add file").click();

    // Wait for hidden file input to appear in the DOM
    cy.get('input[type="file"]', { timeout: 10000 }).should("exist");

    // Path relative to cypress/fixtures
    const filePath = "Sample.pdf";

    // Attach file directly to hidden input
    cy.get('input[type="file"]').attachFile(filePath);

    // (Optional) Assert successful upload
    cy.contains("Upload complete").should("be.visible");
    cy.contains("span.v-btn__content", "Save").click();
  });
});
