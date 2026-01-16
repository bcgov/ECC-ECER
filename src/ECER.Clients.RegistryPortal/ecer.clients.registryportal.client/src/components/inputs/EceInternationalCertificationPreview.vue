<template>
  <PreviewCard
    title="International certification"
    portal-stage="InternationalCertification"
  >
    <template #content>
      <div
        v-for="(internationalCertificate, index) in internationalCertificates"
        :key="internationalCertificate.id!"
      >
        <v-divider
          v-if="index !== 0"
          :thickness="2"
          color="grey-lightest"
          class="border-opacity-100 my-6"
        />
        <v-row>
          <v-col cols="4">
            <p class="small">Country of Institution</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                `${configStore.countryName(internationalCertificate.countryId || "")}`
              }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Name of regulatory authority</p>
          </v-col>
          <v-col>
            <p id="courseProvince" class="small font-weight-bold">
              {{ internationalCertificate.nameOfRegulatoryAuthority }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Email of regulatory authority</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ internationalCertificate.emailOfRegulatoryAuthority }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Phone number of regulatory authority (optional)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                `${internationalCertificate.phoneOfRegulatoryAuthority || "—"}`
              }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Website of regulatory authority (optional)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              <a
                v-if="internationalCertificate?.websiteOfRegulatoryAuthority"
                :href="internationalCertificate.websiteOfRegulatoryAuthority"
                target="_blank"
              >
                {{ internationalCertificate.websiteOfRegulatoryAuthority }}
              </a>
              <span v-else>{{ "—" }}</span>
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">
              Online certificate validation tool of regulatory authority
              (optional)
            </p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              <a
                v-if="
                  internationalCertificate?.onlineCertificateValidationToolOfRegulatoryAuthority
                "
                :href="
                  internationalCertificate.onlineCertificateValidationToolOfRegulatoryAuthority
                "
                target="_blank"
              >
                {{
                  internationalCertificate.onlineCertificateValidationToolOfRegulatoryAuthority
                }}
              </a>
              <span v-else>{{ "—" }}</span>
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Certification status</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ internationalCertificate.certificateStatus }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Certificate title</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ internationalCertificate.certificateTitle }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Issue date</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                `${formatDate(internationalCertificate.issueDate || "", "LLLL d, yyyy")}`
              }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Expiry date (if applicable to your certificate)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                internationalCertificate?.expiryDate
                  ? formatDate(
                      internationalCertificate.expiryDate || "",
                      "LLLL d, yyyy",
                    )
                  : "—"
              }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Name on certificate</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ fullName(internationalCertificate) }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Copy of certificate</p>
          </v-col>
          <v-col>
            <v-row no-gutters>
              <v-col
                v-for="(file, childIndex) in internationalCertificate.files"
                :key="childIndex"
                cols="12"
                class="small font-weight-bold"
              >
                {{ file.name }}
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </div>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { formatDate } from "@/utils/format";
import type { Components } from "@/types/openapi";
import { useWizardStore } from "@/store/wizard";
import { useConfigStore } from "@/store/config";

export default defineComponent({
  name: "EceInternationalCertificationPreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();

    return {
      wizardStore,
      configStore,
      formatDate,
    };
  },
  methods: {
    fullName(
      internationalCertificate: Components.Schemas.InternationalCertification,
    ): string {
      return `${internationalCertificate?.otherFirstName || ""} ${internationalCertificate?.otherMiddleName || ""} ${internationalCertificate?.otherLastName || ""}`.trim();
    },
  },
  computed: {
    internationalCertificates(): Components.Schemas.InternationalCertification[] {
      return this.wizardStore.wizardData[
        this.wizardStore.wizardConfig.steps?.internationalCertification?.form
          ?.inputs?.internationalCertification?.id || ""
      ];
    },
  },
});
</script>
