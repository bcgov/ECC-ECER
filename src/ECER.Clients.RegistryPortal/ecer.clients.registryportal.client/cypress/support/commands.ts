import selectors from "../support/selectors";
// Custom command to reset state (clear cookies, local storage, and session storage)
Cypress.Commands.add("resetBrowserState", () => {
  cy.clearCookies();
  cy.clearLocalStorage();
  // Clear session storage if your app uses it
  cy.window().then((win) => {
    win.sessionStorage.clear();
  });
});

// Custom command to log in using default or provided credentials.
Cypress.Commands.add("login", (username?: string, password?: string) => {
  const user = username ?? (Cypress.env("PORTAL_USER").BCSC_USERNAME as string);
  const pass = password ?? (Cypress.env("PORTAL_USER").BCSC_PASSWORD as string);

  // Adjust the URL if login page is at a different route.
  cy.visit("/login");
  // Perform login steps:
  // 1. Click the button with the text "Log in with BC Services Card".
  cy.contains("button", "Log in with BC Services Card").should("be.visible").click();

  cy.origin("https://idtest.gov.bc.ca", { args: { user, pass } }, ({ user, pass }) => {
    // 2. Click on the tile (or button) with the given id.
    cy.get("#tile_test_with_username_password_device_div_id").click({ force: true });
    // 3. Enter the username and password.
    cy.get('input[name="username"]').type(user, { force: true, log: false });
    cy.get('input[name="password"]').type(pass, { force: true, log: false });
    // 4. Click the submit button.
    cy.get("#submit-btn").click({ force: true });
    // 5. Check "I agree to the BC Login Service Terms of Use"
    cy.get('input[id="accept"]').click({ force: true });
    // 6. Click the "Continue" button.
    cy.get('button[id="btnSubmit"').click({ force: true });
  });

  // 5. Verify that the login was successful.
  cy.url().should("not.include", "/login");
  cy.document().its("readyState").should("eq", "complete");
  cy.contains("My current certification").should("be.visible");
});

// Custom command to log out of the ECER portal.
Cypress.Commands.add("logout", () => {
  cy.get("body").then(($body: JQuery<HTMLElement>) => {
    // 1) check for username button
    const $userBtn = $body.find(selectors.navigationBar.userNameButton);
    if ($userBtn.length > 0 && $userBtn.is(":visible")) {
      cy.wrap($userBtn).click();
      // click logout link
      return cy.get(selectors.navigationBar.logOutLink).click();
    }
  });
});

Cypress.Commands.add("resetUserState", () => {
  const baseApiUrl: string = Cypress.env("API_URL").replace(/\/$/, "");
  const apiKey: string = Cypress.env("API_KEY");
  const externalUserId: string = Cypress.env("PORTAL_USER").EXTERNAL_USER_ID;

  return cy
    .request({
      method: "DELETE",
      url: `${baseApiUrl}/api/E2ETests/user/reset`,
      headers: { "X-API-KEY": apiKey, "EXTERNAL-USER-ID": externalUserId },
      failOnStatusCode: true,
      retryOnStatusCodeFailure: true,
      retryOnNetworkFailure: true,
    })
    .then((response) => {
      expect(response.status).to.eq(200);
      cy.log("User State Reset sucessfull");
      // Return the response if needed for further chaining
      return cy.wrap(response);
    });
});

Cypress.Commands.add("seedRenewalApplication", (applicationType?: string, IsActive?: boolean, IsExpiredMoreThan5Years?: boolean) => {
  const baseApiUrl: string = Cypress.env("API_URL").replace(/\/$/, "");
  const apiKey: string = Cypress.env("API_KEY");
  const externalUserId: string = Cypress.env("PORTAL_USER").EXTERNAL_USER_ID;

  return cy
    .request({
      method: "POST",
      url: `${baseApiUrl}/api/E2ETests/applications/seed/renewal`,
      headers: {
        "X-API-KEY": apiKey,
        "EXTERNAL-USER-ID": externalUserId,
        "APPLICATION-TYPE": applicationType,
        "APP-STATUS": IsActive,
        "APP-EXPIRATION": IsExpiredMoreThan5Years,
      },
      failOnStatusCode: true,
      retryOnStatusCodeFailure: true,
      retryOnNetworkFailure: true,
    })
    .then((response) => {
      expect(response.status).to.eq(200);
      cy.log("Applicaion and Certification seeded sucessfully");
      // Return the response if needed for further chaining
      return cy.wrap(response);
    });
});

/* Custom command to wait for a button to be ready (not loading) before clicking
  This is useful for Vuetify buttons that have a loading state */

Cypress.Commands.overwrite("click", (originalFn, subject, options) => {
  const $el = subject as unknown as JQuery<HTMLElement>;

  // If it's not a Vuetify button, do the normal click and exit
  if (!$el.hasClass("v-btn")) {
    return originalFn(subject, options);
  }

  // Otherwise wait for it to be visible & spinner-free, then click
  return cy
    .wrap(subject)
    .should("be.visible")
    .should("not.have.class", "v-btn--loading")
    .then(($el) => {
      // **return** the real click so the Cypress chain stays intact
      return originalFn($el, options);
    });
});

// overwrite the built-in 'type' to always include force: true
Cypress.Commands.overwrite("type", (...args: any[]) => {
  const [originalFn, subject, text, options = {}] = args;
  return originalFn(subject, text, { force: true, ...options });
});
export {};
