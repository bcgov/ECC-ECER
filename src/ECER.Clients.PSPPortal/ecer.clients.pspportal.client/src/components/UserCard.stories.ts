import type { Meta, StoryObj } from "@storybook/vue3-vite";

import UserCard from "./UserCard.vue";

const meta = {
  title: "Example/UserCard",
  component: UserCard,
  tags: ["autodocs"],
  argTypes: {},
  args: {
    currentUserId: "00000000-0000-0000-0000-000000000000",
    user: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "John",
        lastName: "Doe",
        email: "john.doe@example.com",
        jobTitle: "Program Coordinator",
        role: "Primary",
      },
      accessToPortal: "Active",
    },
  },
} satisfies Meta<typeof UserCard>;

export default meta;
type Story = StoryObj<typeof meta>;

/*
 *👇 Render functions are a framework specific feature to allow you control on how the component renders.
 * See https://storybook.js.org/docs/api/csf
 * to learn how to use render functions.
 */

export const ActivePrimary: Story = {
  args: {
    currentUserId: "00000000-0000-0000-0000-000000000000",
    user: {
      id: "f403a278-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "Jane",
        lastName: "Smith",
        email: "jane.smith@example.com",
        jobTitle: "Program Director",
        role: "Primary",
      },
      accessToPortal: "Active",
    },
  },
};

export const ActiveSecondary: Story = {
  args: {
    currentUserId: "00000000-0000-0000-0000-000000000000",
    user: {
      id: "a1b2c3d4-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "Bob",
        lastName: "Johnson",
        email: "bob.johnson@example.com",
        jobTitle: "Administrative Assistant",
        role: "Secondary",
      },
      accessToPortal: "Active",
    },
  },
};

export const ActiveSecondarySameUser: Story = {
  args: {
    currentUserId: "a1b2c3d4-8020-f011-998a-6045bdf9b81b",
    user: {
      id: "a1b2c3d4-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "Bob",
        lastName: "Johnson",
        email: "bob.johnson@example.com",
        jobTitle: "Administrative Assistant",
        role: "Secondary",
      },
      accessToPortal: "Active",
    },
  },
};

export const Invited: Story = {
  args: {
    currentUserId: "00000000-0000-0000-0000-000000000000",
    user: {
      id: "e5f6g7h8-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "Alice",
        lastName: "Williams",
        email: "alice.williams@example.com",
        jobTitle: "Program Coordinator",
        role: "Secondary",
      },
      accessToPortal: "Invited",
    },
  },
};

export const Disabled: Story = {
  args: {
    currentUserId: "00000000-0000-0000-0000-000000000000",
    user: {
      id: "i9j0k1l2-8020-f011-998a-6045bdf9b81b",
      profile: {
        firstName: "Charlie",
        lastName: "Brown",
        email: "charlie.brown@example.com",
        jobTitle: "Former Coordinator",
        role: "Secondary",
      },
      accessToPortal: "Disabled",
    },
  },
};
