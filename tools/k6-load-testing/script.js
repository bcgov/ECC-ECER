import { sleep, check } from "k6";
import http from "k6/http";

// Peak requests per minutes we've seen on the system
const BASELINE_RATE = 25;

const MAX_ALLOWED_FAILURE_RATE = "0.01"; // http errors should be less than 1%

export const options = {
  //only run one scenario at a time!
  scenarios: {
    rampUpProfile: {
      executor: "ramping-arrival-rate", //Assure load increase if the system slows
      startRate: 1,
      timeUnit: "1m",
      preAllocatedVUs: 500, //if we use cloud - we are limited to 100, but if you run it locally you can raise this number higher
      stages: [
        { duration: "1m", target: BASELINE_RATE }, // just slowly ramp-up
        { duration: "1m", target: BASELINE_RATE * 2 },
        { duration: "1m", target: BASELINE_RATE * 4 },
        { duration: "1m", target: BASELINE_RATE * 6 },
        { duration: "1m", target: BASELINE_RATE * 8 },
        { duration: "1m", target: BASELINE_RATE * 10 },
        { duration: "2m", target: BASELINE_RATE * 12 },
        { duration: "2m", target: BASELINE_RATE * 14 },
        { duration: "2m", target: BASELINE_RATE * 16 },
        { duration: "2m", target: BASELINE_RATE * 20 },
        { duration: "2m", target: BASELINE_RATE * 40 },
        { duration: "2m", target: BASELINE_RATE * 80 },
      ],
      tags: {
        profile: "soak",
      },
    },
  },
  thresholds: {
    http_req_failed: [
      {
        threshold: `rate<${MAX_ALLOWED_FAILURE_RATE}`,
        // Leave this in! Don't keep hammering the poor server after its failing, requests will queue
        abortOnFail: true,
      },
    ],
  },
};
const CHARACTER_REFERENCE_INVITE_TOKEN =
  "<<Add in verify token here>>";

//this will change based on ENV
const BASE_ROUTE =
  "https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca";

//Note you will need to change the recaptcha token to a TEST one. Otherwise these recaptcha tokens will fail. Do this by changing the recaptcha tokens on openshift
function getLookupCertification() {
  const payload = {
    firstName: "",
    lastName: "smith",
    recaptchaToken: "fakestring",
    registrationNumber: "",
  };

  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };
  const res = http.post(
    `${BASE_ROUTE}/api/certifications/lookup`,
    JSON.stringify(payload),
    params
  );
  check(res, { "status was 200": (r) => r.status === 200 });
}

function getProvinceList() {
  const res = http.get(`${BASE_ROUTE}/api/provincelist`);
  check(res, { "status was 200": (r) => r.status === 200 });
}

function getConfiguration() {
  const res = http.get(`${BASE_ROUTE}/api/configuration`);
  check(res, { "status was 200": (r) => r.status === 200 });
}

function getPortalInvitationCharacterReference() {
  const res = http.get(
    `${BASE_ROUTE}/api/PortalInvitations/${CHARACTER_REFERENCE_INVITE_TOKEN}`
  );
  check(res, { "status was 200": (r) => r.status === 200 });
}

//this is the 'runner' function - you can call multiple endpoints here
export default function () {
  getLookupCertification();
  getProvinceList();
  getConfiguration();
  getPortalInvitationCharacterReference();
  sleep(1);
}
