import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay, todayDay } from "../../../support/utils";

describe("New ECE One Year Certificate Application", () => {
  it("should create a sucessfull ECE One Year Application", () => {
    /** Dashboard */
    cy.get(selectors.dashboard.applyNowButton).click();

    /** Certification Type */
    cy.get(selectors.certificationType.applyNowEceOneYearButton).click();

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
    cy.get(selectors.education.programNameInput).type(
      "TEST ECE One Year Course",
    );

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
      "Reference Last Name",
    );
    cy.get(selectors.characterReference.firstNameInput).type(
      "Reference First Name",
    );
    cy.get(selectors.characterReference.emailInput).type(
      "Character_Reference@test.gov.bc.ca",
    );
    cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Application Review and Submit */
    cy.document().its("readyState").should("eq", "complete");
    cy.contains("Review and submit").should("be.visible");
    cy.get(selectors.applicationPreview.certificationType)
      .should("be.visible")
      .should("contain.text", "ECE One Year");
    cy.get(selectors.applicationPreview.characterReferenceFirstName)
      .should("be.visible")
      .should("contain.text", "Reference First Name");
    cy.get(selectors.applicationPreview.characterReferenceLastName)
      .should("be.visible")
      .should("contain.text", "Reference Last Name");
    cy.get(selectors.applicationPreview.characterReferenceEmail)
      .should("be.visible")
      .should("contain.text", "Character_Reference@test.gov.bc.ca");

    cy.get(selectors.applicationPreview.educationCountry)
      .should("be.visible")
      .should("contain.text", "Canada");

    cy.get(selectors.applicationPreview.educationProvince)
      .should("be.visible")
      .should("contain.text", "British Columbia");
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
    cy.contains(
      "p.text-links",
      "Character reference provided by Reference First Name Reference Last Name",
    ).click();
    // cy.get('body').then($body => {
    //   if ($body.find('p.text-links:contains("WorkReference")').length > 0) {
    //     // Element exists → click it
    //     cy.contains('p.text-links', 'WorkReference').click();
    //   } else {
    //     // Element not found → fallback
    //     cy.wait(80000);
    //     cy.contains('a', 'Back to application summary').click();
    //   }
    // });
    // cy.contains('p.text-links', 'Character reference provided by Reference First Name Reference Last Name').click()
    // cy.contains('p.text-links', 'WorkReferenceFirstName WorkReferenceLastName').click();
    cy.contains("a", "Choose a new reference").click({ force: true });

    cy.get('[aria-label="Last Name"]').type("CharacterReferenceLastName");
    cy.get('[aria-label="First Name"]').type("CharacterReferenceLastName");
    cy.get('[aria-label="Email"]').type(
      "CharacterExperience_Reference@test.gov.bc.ca",
    );
    cy.get('[aria-label="Phone Number (optional)"]').type("1234567890");
    // cy.get('[aria-label="Work experience hours observed by reference"]').type("500");
    cy.contains("span.v-btn__content", "Save new reference").click();
  });
});
