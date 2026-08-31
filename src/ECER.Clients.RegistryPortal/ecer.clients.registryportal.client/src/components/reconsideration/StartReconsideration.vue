<template>
  <Loading v-if="loadingStore.isLoading('reconsiderations_get')"></Loading>
  <template v-else>
    <v-container fluid class="bg-primary">
      <v-container>
        <v-row>
          <v-col>
            <h2 class="text-white">
              Dispute application for certification decision
            </h2>
          </v-col>
        </v-row>
      </v-container>
    </v-container>
    <PageContainer :margin-top="false">
      <Breadcrumb />

      <h2>Instructions</h2>
      <br />
      <p>Complete the form to dispute the following decisions:</p>
      <ul class="ml-10">
        <li>Intent to refuse to issue your certification</li>

        <li>Intent to reject your renewal application</li>
        <li>Intent to renew your certificate with terms and conditions</li>
        <li>Intent to suspend or cancel your certificate</li>
        <li>
          Intent to attach terms and conditions or vary terms and conditions on
          your certificate
        </li>
        <li>
          Immediate action taken on your certificate (suspending, attaching
          terms and conditions, or varying terms and conditions on your
          certificate)
        </li>
      </ul>
      <br />
      <p>
        This form is not saved until submitted. If you leave the form without
        submitting you can restart it later.
      </p>
      <br />
      <p>You must submit this form by {{ disputeEndDate }}.</p>

      <br />
      <p>
        Attach any additional evidence and/or information, not previously
        submitted that you want the ECE Registry to consider.
      </p>
      <br />
      <h2>Applicable Legislation and Standards</h2>
      <br />
      <p>
        These documents outline the training, education, and other requirements
        for receiving and maintaining certification as an ECE or ECE Assistant
        in B.C. as well as the legislative (legal) requirements related to the
        dispute process.
      </p>
      <br />
      <p>
        The
        <a
          target="_blank"
          href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/00_02075_01"
        >
          Community Care and Assisted Living Act
        </a>
        (CCALA), and the
        <a
          target="_blank"
          href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/332_2007"
        >
          Child Care Licensing Regulation
        </a>
        (CCLR) outline the legislative (legal) requirements that must be met to
        be certified and maintain certification as an ECE or ECE Assistant.
      </p>
      <br />
      <p>
        Section 33 of the CCLR identifies the types of decisions that can be
        disputed and describes the dispute process.
      </p>
      <br />
      <h2>Occupational competencies</h2>
      <br />
      <p>
        The Child Care Sector Occupational Competencies set the standards for
        certification in B.C. To qualify for certification and maintain
        certification, an ECE must demonstrate the competencies in this
        document. ECE Assistants must demonstrate the competencies applicable to
        the courses they have taken to date.
      </p>
      <v-btn class="mt-4" color="primary" @click="handleBeginDispute">
        Begin dispute
      </v-btn>
    </PageContainer>
  </template>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import type { PropType } from "vue";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";
import { formatDate } from "@/utils/format";
import { getReconsiderationsQuery } from "@/api/reconsideration";

import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import Loading from "@/components/Loading.vue";
import type { ReconsiderationType } from "@/types/reconsideration";

export default defineComponent({
  name: "StartReconsideration",
  components: { PageContainer, Breadcrumb, ECEHeader, Loading },
  setup() {
    const router = useRouter();
    const loadingStore = useLoadingStore();

    return { router, loadingStore };
  },
  props: {
    reconsiderationId: { type: String, required: true },
    reconsiderationType: {
      type: String as PropType<ReconsiderationType>,
    },
  },
  async mounted() {
    if (this.reconsiderationType === "application") {
      this.reconsideration = (
        await getReconsiderationsQuery(this.reconsiderationId)
      )?.data?.[0];
    }
  },
  data() {
    return {
      reconsideration: undefined as
        Components.Schemas.Reconsideration | undefined,
    };
  },
  methods: {
    handleBeginDispute() {
      this.router.push({
        name: "reconsideration",
        params: {
          reconsiderationId: this.reconsiderationId,
          reconsiderationType: this.reconsiderationType,
        },
      });
    },
  },
  computed: {
    disputeEndDate(): string {
      if (this.reconsiderationType === "application") {
        return formatDate(
          this.reconsideration?.reconsiderationEndDate || "",
          "LLLL d, yyyy",
        );
      } else if (this.reconsiderationType === "investigation") {
        // TODO implement with investigations
        return "not implemented";
      }
      console.warn(
        `unhandled reconsideration type: ${this.reconsiderationType}`,
      );
      return "unknown";
    },
  },
});
</script>
