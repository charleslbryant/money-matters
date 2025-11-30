import type { Meta, StoryObj } from '@storybook/react';
import { FormProvider, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { NumberField } from './NumberField';

// Wrapper component to provide form context
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const NumberFieldWrapper = (args: any) => {
  const schema = z.object({
    [args.name]: z.number().min(0.01, 'Amount must be greater than 0'),
  });

  const methods = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues: { [args.name]: args.defaultValue },
  });

  return (
    <FormProvider {...methods}>
      <form className="w-96">
        <NumberField {...args} />
      </form>
    </FormProvider>
  );
};

const meta = {
  title: 'Forms/NumberField',
  component: NumberField,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  render: (args) => <NumberFieldWrapper {...args} />,
} satisfies Meta<typeof NumberField>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    name: 'amount',
    label: 'Amount',
    placeholder: '0.00',
  },
};

export const WithCurrencySuffix: Story = {
  args: {
    name: 'amount',
    label: 'Amount',
    placeholder: '0.00',
    suffix: '$',
    required: true,
  },
};

export const WithPrefix: Story = {
  args: {
    name: 'amount',
    label: 'Price',
    placeholder: '0.00',
    prefix: '$',
    required: true,
  },
};

export const WithHelperText: Story = {
  args: {
    name: 'amount',
    label: 'Transaction Amount',
    placeholder: '0.00',
    suffix: '$',
    helperText: 'Enter the total transaction amount',
    required: true,
  },
};

export const WithMinMax: Story = {
  args: {
    name: 'percentage',
    label: 'Percentage',
    placeholder: '0',
    suffix: '%',
    min: 0,
    max: 100,
    step: 1,
    helperText: 'Enter a value between 0 and 100',
  },
};

export const Disabled: Story = {
  args: {
    name: 'amount',
    label: 'Amount',
    placeholder: '0.00',
    suffix: '$',
    disabled: true,
    defaultValue: 1234.56,
  },
};

export const Integer: Story = {
  args: {
    name: 'quantity',
    label: 'Quantity',
    placeholder: '0',
    step: 1,
    min: 1,
    helperText: 'Enter a whole number',
  },
};
