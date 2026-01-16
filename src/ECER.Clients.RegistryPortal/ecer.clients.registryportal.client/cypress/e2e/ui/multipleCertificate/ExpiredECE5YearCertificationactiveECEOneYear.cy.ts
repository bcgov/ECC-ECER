import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay } from "../../../support/utils";

describe(" Expired  ECE 5 Year Certificate Application Active One Yaar ", () => {
  it(" ECE Five Year be Visbile", () => {
    cy.seedRenewalApplication("ECE5Years", false, false);

    cy.reload();
    /** Dashboard */
    cy.seedRenewalApplication("ECEOneYear", false, false);
    //  Validate Only 5 year certificate is visible
    cy.reload();
    cy.contains("Early Childhood Educator - ECE Five Year").should("be.visible");
  });
});
