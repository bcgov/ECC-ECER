import { getClient } from "@/api/client";
import type { Components, Paths } from "@/types/openapi";

const getConfiguration = async (): Promise<
  Components.Schemas.ApplicationConfiguration | null | undefined
> => {
  const client = await getClient(false);
  return (await client.configuration_get()).data;
};

const getProvinceList = async (): Promise<
  Components.Schemas.Province[] | null | undefined
> => {
  const client = await getClient(false);
  return (await client.province_get()).data;
};

const getCountryList = async (): Promise<
  Components.Schemas.Country[] | null | undefined
> => {
  const client = await getClient(false);
  return (await client.country_get()).data;
};

const getAreaOfInstructionList = async (): Promise<
  Components.Schemas.AreaOfInstruction[] | null | undefined
> => {
  const client = await getClient(false);
  return (await client.area_of_instruction_get()).data.areaOfInstruction;
};

export {
  getConfiguration,
  getProvinceList,
  getCountryList,
  getAreaOfInstructionList,
};
