import type { Meta, StoryObj } from '@storybook/react';
import { FormProvider, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Select } from './Select';

const categoryOptions = [
  { value: 'income', label: 'Income' },
  { value: 'expense', label: 'Expense' },
  { value: 'transfer', label: 'Transfer' },
];

const frequencyOptions = [
  { value: 'once', label: 'One-time' },
  { value: 'weekly', label: 'Weekly' },
  { value: 'monthly', label: 'Monthly' },
  { value: 'yearly', label: 'Yearly' },
];

// Wrapper component to provide form context
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const SelectWrapper = (args: any) => {
  const schema = z.object({
    [args.name]: z.string().min(1, 'Please select an option'),
  });

  const methods = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues: { [args.name]: args.defaultValue || '' },
  });

  return (
    <FormProvider {...methods}>
      <form className="w-96">
        <Select {...args} />
      </form>
    </FormProvider>
  );
};

const meta = {
  title: 'Forms/Select',
  component: Select,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  render: (args) => <SelectWrapper {...args} />,
} satisfies Meta<typeof Select>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    name: 'category',
    label: 'Category',
    placeholder: 'Select a category',
    options: categoryOptions,
  },
};

export const Required: Story = {
  args: {
    name: 'category',
    label: 'Category',
    placeholder: 'Select a category',
    options: categoryOptions,
    required: true,
  },
};

export const WithHelperText: Story = {
  args: {
    name: 'frequency',
    label: 'Payment Frequency',
    placeholder: 'Select frequency',
    options: frequencyOptions,
    helperText: 'How often this payment occurs',
    required: true,
  },
};

export const WithPreselectedValue: Story = {
  args: {
    name: 'category',
    label: 'Category',
    options: categoryOptions,
    defaultValue: 'expense',
  },
};

export const Disabled: Story = {
  args: {
    name: 'category',
    label: 'Category',
    options: categoryOptions,
    disabled: true,
    defaultValue: 'income',
  },
};

export const WithDisabledOptions: Story = {
  args: {
    name: 'category',
    label: 'Category',
    placeholder: 'Select a category',
    options: [
      { value: 'income', label: 'Income' },
      { value: 'expense', label: 'Expense' },
      { value: 'transfer', label: 'Transfer (Coming Soon)', disabled: true },
    ],
  },
};
