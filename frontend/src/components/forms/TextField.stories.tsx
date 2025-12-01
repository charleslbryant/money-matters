import type { Meta, StoryObj } from '@storybook/react';
import { FormProvider, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { userEvent, within, expect } from '@storybook/test';
import { TextField } from './TextField';

// Wrapper component to provide form context
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const TextFieldWrapper = (args: any) => {
  const schema = z.object({
    [args.name]: z.string().email('Invalid email address'),
  });

  const methods = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues: { [args.name]: args.defaultValue || '' },
  });

  // Trigger validation if we want to show error state
  if (args.showError) {
    methods.trigger(args.name);
  }

  return (
    <FormProvider {...methods}>
      <form className="w-96">
        <TextField {...args} />
      </form>
    </FormProvider>
  );
};

const meta = {
  title: 'Forms/TextField',
  component: TextField,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  render: (args) => <TextFieldWrapper {...args} />,
  argTypes: {
    type: {
      control: 'select',
      options: ['text', 'email', 'password', 'tel', 'url'],
    },
  },
} satisfies Meta<typeof TextField>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
  },
};

export const Required: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
    required: true,
  },
};

export const WithHelperText: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
    helperText: "We'll never share your email with anyone",
    required: true,
  },
};

export const Disabled: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
    disabled: true,
    defaultValue: 'disabled@example.com',
  },
};

export const Password: Story = {
  args: {
    name: 'password',
    label: 'Password',
    type: 'password',
    placeholder: 'Enter your password',
    required: true,
  },
};

export const Telephone: Story = {
  args: {
    name: 'phone',
    label: 'Phone Number',
    type: 'tel',
    placeholder: '(555) 123-4567',
    helperText: 'Enter your phone number with area code',
  },
};

/**
 * Story with interaction tests - demonstrates user interactions
 * This story includes a play function that simulates user behavior
 */
export const WithInteractions: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
    required: true,
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Find the input field by label
    const emailInput = canvas.getByLabelText(/email address/i);

    // Type valid email
    await userEvent.type(emailInput, 'test@example.com');

    // Verify the value was entered
    await expect(emailInput).toHaveValue('test@example.com');

    // Clear the field
    await userEvent.clear(emailInput);

    // Type invalid email and blur to trigger validation
    await userEvent.type(emailInput, 'invalid-email');
    await userEvent.tab();

    // The validation error should appear (async, so we wait)
    await new Promise((resolve) => setTimeout(resolve, 100));
  },
};

/**
 * Story testing disabled state - user cannot interact
 */
export const DisabledWithInteractions: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    disabled: true,
    defaultValue: 'readonly@example.com',
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Find the input
    const emailInput = canvas.getByLabelText(/email address/i);

    // Verify it's disabled
    await expect(emailInput).toBeDisabled();

    // Verify the default value is set
    await expect(emailInput).toHaveValue('readonly@example.com');
  },
};
