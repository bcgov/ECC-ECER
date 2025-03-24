export default {
  dashboard: {
    applyNowButton: 'button[id="btnApplyNow"]',
  },
  certificationType: {
    eceAssistantRadio: 'input[value="EceAssistant"]',
    eceFiveYearRadio: 'input[value="FiveYears"]',
    eceOneYearRadio: 'input[value="OneYear"]',
    continueButton: 'button[id="btnContinue"]',
  },
  applicationRequirements: {
    applyNowButton: 'button[id="btnApplyNow"]',
  },
  declaration: {
    declarationCheckbox: 'input[id="chkDeclaration"]',
    continueButton: 'button[id="btnContinue"]',
  },
  applicationWizard: {
    saveAndContinueButton: 'button[id="btnSaveAndContinue"]',
    submitApplicationButton: 'button[id="btnSubmitApplication"]',
  },
  education: {
    addEducationButton: 'button[id="btnAddEducation"]',
    transcriptStatusRadioDiv: 'div[aria-describedby="radioTranscriptStatus-messages"]',
    programNameInput: 'input[id="txtProgramName"]',
    programStartDateInput: 'input[id="txtProgramStartDate"]',
    programEndDateInput: 'input[id="txtProgramEndDate"]',
    provinceDropDownList: 'input[id="ddlProvince"]',
    postSecondaryInstitutionDropDownList: 'input[id="ddlPostSecondaryInstitution"]',
    institutionNameInput: 'input[id="txtInstitutionName"]',
    studentIDInput: 'input[id="txtStudentID"]',
    nameOnTranscriptRadioDiv: 'div[aria-describedby="radioNameOnTranscript-messages"]',
    saveEducationButton: 'button[id="btnSaveEducation"]',
  },
  characterReference: {
    lastNameInput: 'input[id="txtReferenceLastName"]',
    firstNameInput: 'input[id="txtReferenceFirstName"]',
    emailInput: 'input[id="txtReferenceEmail"]',
    phoneNumberInput: 'input[id="txtReferencePhoneNumber"]',
  },
  workExperienceReference: {
    addReferenceButton: 'button[id="btnAddWorkExperienceReference"]',
    lastNameInput: 'input[id="txtWorkReferenceLastName"]',
    firstNameInput: 'input[id="txtWorkReferenceFirstName"]',
    emailInput: 'input[id="txtWorkReferenceEmail"]',
    phoneNumberInput: 'input[id="txtWorkReferencePhoneNumber"]',
    hoursInput: 'input[id="txtWorkReferenceHours"]',
    saveReferenceButton: 'button[id="btnSaveWorkReference"]',
  },
  datePicker: {
    monthDiv: "div.v-date-picker-month",
  },
  applicationPreview: {
    certificationType: 'p[id="certificationType"]',
    educationCountry: 'p[id="educationCountry"]',
    educationProvince: 'p[id="educationProvince"]',
    characterReferenceFirstName: 'p[id="characterReferenceFirstName"]',
    characterReferenceLastName: 'p[id="characterReferenceLastName"]',
    characterReferenceEmail: 'p[id="characterReferenceEmail"]',
    workReferenceEmail: 'p[id="workReferenceEmail"]',
    workReferenceName: 'h4[id="workReferenceName"]',
  },
  applicationSubmitted: {
    pageTitle: 'h1[id="titleApplicationSubmitted"]',
    applicationSummaryButton: 'button[id="btnApplicationSummary"]',
  },
  elementType: {
    radio: 'input[type="radio"]',
    input: "input",
    body: "body",
    button: "button",
  },
};
