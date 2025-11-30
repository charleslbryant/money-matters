# Form Components

Comprehensive form handling components built with React Hook Form and Zod validation.

## Overview

This package provides a complete form management solution with:
- Type-safe form handling using React Hook Form
- Schema-based validation using Zod
- Accessible form fields with ARIA attributes
- Consistent error handling and display
- Dark mode support
- Real-time validation feedback

## Components

### Form

The main form wrapper that provides validation context using React Hook Form and Zod.

**Props:**
- `children: React.ReactNode` - Form content
- `onSubmit: SubmitHandler<T>` - Form submission handler
- `schema: ZodSchema<T>` - Zod validation schema
- `defaultValues?: Partial<T>` - Initial form values
- `className?: string` - Additional CSS classes
- `id?: string` - Form element ID

**Example:**
```tsx
import { z } from 'zod';
import { Form, TextField, NumberField } from '@/components/forms';

const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  amount: z.number().min(0.01, 'Amount must be positive'),
});

type FormData = z.infer<typeof schema>;

function MyForm() {
  const handleSubmit = (data: FormData) => {
    console.log('Form data:', data);
  };

  return (
    <Form<FormData>
      schema={schema}
      onSubmit={handleSubmit}
      defaultValues={{ name: '', amount: 0 }}
    >
      <TextField name="name" label="Name" required />
      <NumberField name="amount" label="Amount" required />
      <button type="submit">Submit</button>
    </Form>
  );
}
```

### TextField

Text input field with validation support.

**Props:**
- `name: string` - Field name (must match schema)
- `label?: string` - Field label
- `placeholder?: string` - Placeholder text
- `helperText?: string` - Helper text shown below input
- `required?: boolean` - Show required indicator
- `disabled?: boolean` - Disable the field
- `type?: 'text' | 'email' | 'password' | 'tel' | 'url'` - Input type (default: 'text')
- `autoComplete?: string` - Autocomplete attribute
- `className?: string` - Additional CSS classes for wrapper
- `inputClassName?: string` - Additional CSS classes for input

**Example:**
```tsx
<TextField
  name="email"
  label="Email Address"
  type="email"
  placeholder="you@example.com"
  required
  helperText="We'll never share your email"
/>
```

### NumberField

Numeric input field with currency/prefix support.

**Props:**
- `name: string` - Field name (must match schema)
- `label?: string` - Field label
- `placeholder?: string` - Placeholder text
- `helperText?: string` - Helper text shown below input
- `required?: boolean` - Show required indicator
- `disabled?: boolean` - Disable the field
- `min?: number` - Minimum value
- `max?: number` - Maximum value
- `step?: number` - Step increment (default: 0.01)
- `prefix?: string` - Text shown before input
- `suffix?: string` - Text shown after input (default: '$')
- `className?: string` - Additional CSS classes for wrapper
- `inputClassName?: string` - Additional CSS classes for input

**Example:**
```tsx
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
```

### Select

Dropdown select field with validation.

**Props:**
- `name: string` - Field name (must match schema)
- `label?: string` - Field label
- `placeholder?: string` - Placeholder option text
- `helperText?: string` - Helper text shown below select
- `required?: boolean` - Show required indicator
- `disabled?: boolean` - Disable the field
- `options: SelectOption[]` - Array of options
- `className?: string` - Additional CSS classes for wrapper
- `selectClassName?: string` - Additional CSS classes for select

**SelectOption interface:**
```tsx
interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}
```

**Example:**
```tsx
const categoryOptions = [
  { value: 'income', label: 'Income' },
  { value: 'expense', label: 'Expense' },
  { value: 'transfer', label: 'Transfer' },
];

<Select
  name="category"
  label="Category"
  placeholder="Select a category"
  required
  options={categoryOptions}
/>
```

### TextArea

Multi-line text input with character count.

**Props:**
- `name: string` - Field name (must match schema)
- `label?: string` - Field label
- `placeholder?: string` - Placeholder text
- `helperText?: string` - Helper text shown below textarea
- `required?: boolean` - Show required indicator
- `disabled?: boolean` - Disable the field
- `rows?: number` - Number of visible rows (default: 4)
- `maxLength?: number` - Maximum character count (shows counter if set)
- `className?: string` - Additional CSS classes for wrapper
- `textAreaClassName?: string` - Additional CSS classes for textarea

**Example:**
```tsx
<TextArea
  name="description"
  label="Description"
  placeholder="Enter notes"
  rows={4}
  maxLength={200}
  helperText="Optional transaction notes"
/>
```

### SearchInput

Standalone search input with clear button (does not require Form wrapper).

**Props:**
- `value?: string` - Controlled value
- `onChange?: (value: string) => void` - Change handler
- `placeholder?: string` - Placeholder text (default: 'Search...')
- `className?: string` - Additional CSS classes
- `onClear?: () => void` - Callback when clear button is clicked
- `showClearButton?: boolean` - Show/hide clear button (default: true)

**Example:**
```tsx
const [search, setSearch] = useState('');

<SearchInput
  value={search}
  onChange={setSearch}
  placeholder="Search transactions..."
/>
```

### Helper Components

#### FormField
Wrapper component for consistent field spacing.

```tsx
<FormField className="mb-6">
  {/* field content */}
</FormField>
```

#### FormLabel
Accessible label with required indicator.

```tsx
<FormLabel htmlFor="email" required>
  Email Address
</FormLabel>
```

#### FormError
Error message display component.

```tsx
<FormError message="This field is required" />
```

#### FormHelperText
Helper text display component.

```tsx
<FormHelperText>
  Enter your email address
</FormHelperText>
```

## Validation

All form fields automatically integrate with Zod validation schemas. Validation runs:
- On field blur (after user leaves field)
- On form submission
- Errors display below the field in red
- Fields with errors get red border styling

**Example validation schema:**
```tsx
import { z } from 'zod';

const billSchema = z.object({
  name: z.string()
    .min(1, 'Bill name is required')
    .max(100, 'Name is too long'),

  amount: z.number()
    .min(0.01, 'Amount must be greater than 0')
    .max(999999, 'Amount is too large'),

  dueDate: z.string()
    .min(1, 'Due date is required'),

  frequency: z.enum(['once', 'weekly', 'monthly', 'yearly'], {
    errorMap: () => ({ message: 'Please select a frequency' })
  }),

  notes: z.string().optional(),
});

type BillFormData = z.infer<typeof billSchema>;
```

## Accessibility

All form components include:
- Proper `<label>` associations via `htmlFor`
- ARIA attributes (`aria-invalid`, `aria-describedby`)
- Required field indicators
- Keyboard navigation support
- Screen reader friendly error messages
- Focus management

## Styling

Components use Tailwind CSS with:
- Light and dark mode support
- Consistent spacing and typography
- Focus states with ring indicators
- Disabled states with reduced opacity
- Error states with red highlighting
- Responsive design

## Form State Management

Access form state using React Hook Form hooks:

```tsx
import { useFormContext } from 'react-hook-form';

function MyComponent() {
  const {
    formState: { errors, isSubmitting, isDirty, isValid },
    reset,
    getValues,
  } = useFormContext();

  // Use form state
}
```

## Best Practices

1. **Always define a Zod schema** for type safety and validation
2. **Use TypeScript inference** with `z.infer<typeof schema>` for form types
3. **Provide helpful error messages** in your schema validation
4. **Use appropriate input types** (email, tel, url) for better mobile UX
5. **Add helper text** for complex fields or requirements
6. **Mark required fields** with the `required` prop
7. **Disable submit buttons** during submission to prevent double-submits
8. **Reset forms** after successful submission if needed

## Example: Complete Bill Form

```tsx
import { z } from 'zod';
import { Form, TextField, NumberField, Select, TextArea } from '@/components/forms';

const billSchema = z.object({
  name: z.string().min(1, 'Bill name is required'),
  amount: z.number().min(0.01, 'Amount must be greater than 0'),
  frequency: z.string().min(1, 'Please select a frequency'),
  dueDay: z.number().min(1).max(31),
  notes: z.string().optional(),
});

type BillFormData = z.infer<typeof billSchema>;

export function BillForm() {
  const handleSubmit = async (data: BillFormData) => {
    try {
      await api.post('/bills', data);
      alert('Bill created successfully!');
    } catch (error) {
      console.error('Failed to create bill:', error);
    }
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
        dueDay: 1,
        notes: '',
      }}
    >
      <TextField
        name="name"
        label="Bill Name"
        placeholder="e.g., Electric Bill"
        required
      />

      <NumberField
        name="amount"
        label="Amount"
        placeholder="0.00"
        required
        suffix="$"
      />

      <Select
        name="frequency"
        label="Frequency"
        placeholder="Select frequency"
        required
        options={frequencyOptions}
      />

      <NumberField
        name="dueDay"
        label="Due Day of Month"
        required
        min={1}
        max={31}
        step={1}
        suffix=""
      />

      <TextArea
        name="notes"
        label="Notes"
        placeholder="Optional notes"
        rows={3}
        maxLength={200}
      />

      <div className="flex gap-3">
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-colors"
        >
          Create Bill
        </button>
        <button
          type="reset"
          className="px-4 py-2 bg-gray-200 dark:bg-gray-700 text-gray-900 dark:text-gray-100 rounded-md hover:bg-gray-300 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 transition-colors"
        >
          Reset
        </button>
      </div>
    </Form>
  );
}
```

## Testing

To test the form components, see the `FormExample.tsx` component which demonstrates all features with working validation.

## Dependencies

- `react-hook-form` - Form state management
- `zod` - Schema validation
- `@hookform/resolvers` - Zod integration for React Hook Form
