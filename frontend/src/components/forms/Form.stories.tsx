import type { Meta, StoryObj } from '@storybook/react';
import { z } from 'zod';
import { Form, TextField, NumberField, Select, TextArea } from './index';

const billSchema = z.object({
  name: z.string().min(1, 'Bill name is required').max(100, 'Name is too long'),
  amount: z.number().min(0.01, 'Amount must be greater than 0'),
  frequency: z.string().min(1, 'Please select a frequency'),
  notes: z.string().optional(),
});

type BillFormData = z.infer<typeof billSchema>;

const BillFormComponent = () => {
  const handleSubmit = (data: BillFormData) => {
    console.log('Form submitted:', data);
    alert(
      `Bill created!\n\nName: ${data.name}\nAmount: $${data.amount}\nFrequency: ${data.frequency}\n\nCheck console for full data.`
    );
  };

  const frequencyOptions = [
    { value: 'weekly', label: 'Weekly' },
    { value: 'monthly', label: 'Monthly' },
    { value: 'yearly', label: 'Yearly' },
  ];

  return (
    <Form<BillFormData>
      schema={billSchema}
      onSubmit={handleSubmit}
      defaultValues={{
        name: '',
        amount: 0,
        frequency: '',
        notes: '',
      }}
    >
      <TextField
        name="name"
        label="Bill Name"
        placeholder="e.g., Electric Bill"
        required
        helperText="Enter a descriptive name for this bill"
      />

      <NumberField
        name="amount"
        label="Amount"
        placeholder="0.00"
        required
        suffix="$"
        min={0}
        step={0.01}
        helperText="Typical monthly amount"
      />

      <Select
        name="frequency"
        label="Frequency"
        placeholder="Select frequency"
        required
        options={frequencyOptions}
        helperText="How often this bill is due"
      />

      <TextArea
        name="notes"
        label="Notes"
        placeholder="Optional notes about this bill"
        rows={3}
        maxLength={200}
      />

      <div className="flex gap-3 pt-4">
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors"
        >
          Create Bill
        </button>
        <button
          type="reset"
          className="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-gray-100 rounded-md hover:bg-gray-300 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-colors"
        >
          Reset
        </button>
      </div>
    </Form>
  );
};

const meta = {
  title: 'Forms/Complete Bill Form',
  component: BillFormComponent,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
} satisfies Meta<typeof BillFormComponent>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};
