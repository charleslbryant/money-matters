import React from 'react';
import { z } from 'zod';
import {
  Form,
  TextField,
  NumberField,
  Select,
  TextArea,
  SearchInput,
} from './index';

const exampleSchema = z.object({
  name: z.string().min(1, 'Name is required').max(50, 'Name is too long'),
  email: z.string().email('Invalid email address'),
  amount: z.number().min(0.01, 'Amount must be greater than 0'),
  category: z.string().min(1, 'Please select a category'),
  description: z.string().optional(),
});

type ExampleFormData = z.infer<typeof exampleSchema>;

export function FormExample() {
  const [searchValue, setSearchValue] = React.useState('');

  const handleSubmit = (data: ExampleFormData) => {
    console.log('Form submitted:', data);
    alert('Form submitted successfully! Check console for data.');
  };

  const categoryOptions = [
    { value: 'income', label: 'Income' },
    { value: 'expense', label: 'Expense' },
    { value: 'transfer', label: 'Transfer' },
  ];

  return (
    <div className="max-w-2xl mx-auto p-6 space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">
          Form Components Example
        </h1>
        <p className="text-gray-600 dark:text-gray-400">
          Demonstrating React Hook Form + Zod validation
        </p>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">
          Form with Validation
        </h2>
        <Form<ExampleFormData>
          schema={exampleSchema}
          onSubmit={handleSubmit}
          defaultValues={{
            name: '',
            email: '',
            amount: undefined,
            category: '',
            description: '',
          }}
        >
          <TextField
            name="name"
            label="Full Name"
            placeholder="Enter your name"
            required
            helperText="Your full name as it appears on your account"
          />

          <TextField
            name="email"
            label="Email Address"
            type="email"
            placeholder="you@example.com"
            required
          />

          <NumberField
            name="amount"
            label="Amount"
            placeholder="0.00"
            required
            min={0}
            step={0.01}
            suffix="$"
            helperText="Enter the transaction amount"
          />

          <Select
            name="category"
            label="Category"
            placeholder="Select a category"
            required
            options={categoryOptions}
          />

          <TextArea
            name="description"
            label="Description"
            placeholder="Optional notes about this transaction"
            rows={4}
            maxLength={200}
          />

          <div className="flex gap-3 pt-4">
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors"
            >
              Submit Form
            </button>
            <button
              type="reset"
              className="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-gray-100 rounded-md hover:bg-gray-300 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-colors"
            >
              Reset
            </button>
          </div>
        </Form>
      </div>

      <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">
          Search Input
        </h2>
        <SearchInput
          value={searchValue}
          onChange={setSearchValue}
          placeholder="Search transactions..."
        />
        {searchValue && (
          <p className="mt-3 text-sm text-gray-600 dark:text-gray-400">
            Searching for: <strong>{searchValue}</strong>
          </p>
        )}
      </div>

      <div className="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
        <h3 className="text-sm font-semibold text-blue-900 dark:text-blue-100 mb-2">
          Validation Tips
        </h3>
        <ul className="text-sm text-blue-800 dark:text-blue-200 space-y-1">
          <li>• Try submitting the form empty to see validation errors</li>
          <li>• Enter an invalid email to see field-level validation</li>
          <li>• Enter a negative amount to test number validation</li>
          <li>• All fields show real-time validation on blur</li>
        </ul>
      </div>
    </div>
  );
}
