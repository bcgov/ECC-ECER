<template>
  <Loading v-if="loadingStore.isLoading('reconsiderations_get')"></Loading>
  <p v-else-if="(reconsiderations?.length || 0) <= 0">
    There are no active disputes available
  </p>
  <template v-else>
    <PageContainer :margin-top="false">
      <Breadcrumb />
      <v-row>
        <v-col>
          <h2>My disputes</h2>
        </v-col>
      </v-row>
      <v-row
        v-for="(reconsideration, index) in reconsiderations"
        :key="reconsideration.id || index"
      >
        <v-col>
          <v-card class="card-border" flat>
            <v-card-item>
              <v-row no-gutters>
                <v-col>
                  <strong>
                    Dispute application for certification decision
                  </strong>
                </v-col>
              </v-row>
              <v-row no-gutters>
                <v-col>
                  <v-btn
                    class="mt-4"
                    color="primary"
                    @click="
                      router.push({
                        name: 'start-reconsideration',
                        params: {
                          reconsiderationId: reconsideration.id,
                          reconsiderationType:
                            'application' as ReconsiderationType,
                        },
                      })
                    "
                  >
                    Start dispute
                  </v-btn>
                </v-col>
              </v-row>
            </v-card-item>
          </v-card>
        </v-col>
      </v-row>
    </PageContainer>
  </template>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";
import { getReconsiderationsQuery } from "@/api/reconsideration";

import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import Loading from "@/components/Loading.vue";
import type { ReconsiderationType } from "@/types/reconsideration";

export default defineComponent({
  name: "ViewReconsiderations",
  components: { PageContainer, Breadcrumb, ECEHeader, Loading },
  setup() {
    const router = useRouter();
    const loadingStore = useLoadingStore();

    return { router, loadingStore };
  },
  async mounted() {
    this.reconsiderations = (await getReconsiderationsQuery(undefined, ["New"]))
      ?.data;
  },
  data() {
    return {
      reconsiderations: [] as Components.Schemas.Reconsideration[] | undefined,
    };
  },
  methods: {},
});
</script>
<style lang="scss">
.card-border {
  border-style: solid;
  border-color: black;
  border-width: 1px;

  border-top-width: 5px;
}
</style>
