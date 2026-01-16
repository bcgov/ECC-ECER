import selectors from "../../../../support/selectors";
import { courseStartDay, courseEndDay } from "../../../../support/utils";

describe("Renew Expired ECE 5 Year Plus ITE & SNE Certificate Application", () => {
  it("should sucessfully create a Renewal ECE 5 Year Application", () => {
    cy.seedRenewalApplication("ECE5Years", false, false);

    cy.reload();
    /** Dashboard */
    cy.get(selectors.dashboard.renew).click();

    /** Application Requirements */
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Contact Information */
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Renewal information */
    cy.get(
      selectors.renewalInformation.fiveYearRenewalInformationRadio,
    ).check();
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Professional Development */
    cy.get(selectors.professionalDevelopment.addCourseOrWorkshop)
      .contains("Add course or workshop")
      .click();

    cy.get(selectors.professionalDevelopment.courseNameInput)
      .should("exist")
      .type("ECE 5 year");

    cy.get(selectors.professionalDevelopment.howManyHours)
      .should("exist")
      .type("40");

    cy.get(selectors.professionalDevelopment.nameOfPlace).type(
      "British Columbia",
    );

    /* Start Date - DatePicker*/
    cy.get(selectors.professionalDevelopment.courseStartDateInput).click({
      force: true,
    });
    cy.get(selectors.professionalDevelopment.courseStartDateInput).clear();
    cy.get(selectors.professionalDevelopment.courseStartDateInput).type(
      `${courseStartDay} {enter}`,
    );
    cy.get("body").click(0, 0);

    /* End Date - DatePicker*/
    cy.get(selectors.professionalDevelopment.courseEndDateInput).click({
      force: true,
    });
    cy.get(selectors.professionalDevelopment.courseEndDateInput).clear();
    cy.get(selectors.professionalDevelopment.courseEndDateInput).type(
      `${courseEndDay} {enter}`,
    );

    cy.get(selectors.professionalDevelopment.phoneNoOfInstructorCheckBox).check(
      { force: true },
    );

    cy.get(selectors.professionalDevelopment.nameOfInstructor).type(
      "James Bond",
    );
    cy.get(selectors.professionalDevelopment.PhoneNoOfInstructor).type(
      "6474895555",
    );

    cy.get(selectors.professionalDevelopment.saveCourseOrWorkshop).click();

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
    cy.get(selectors.workExperienceReference.hoursInput).type("400");

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
    cy.get(selectors.applicationPreview.courseProvince)
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
  });
});
