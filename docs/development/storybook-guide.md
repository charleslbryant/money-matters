# Storybook Guide for Money Matters

## Overview

Storybook is a frontend workshop for building UI components in isolation. It provides an interactive development environment for our React components, allowing us to:

- Develop components in isolation without running the full app
- Document component APIs and usage examples
- Test components in different states and with different props
- Create a living style guide for the design system
- Perform visual regression testing
- Share components with designers and stakeholders

## Why Storybook for Money Matters?

### Benefits

1. **Component Development**
   - Build and test components without backend dependencies
   - Iterate quickly on UI without waiting for API endpoints
   - Test edge cases and error states easily

2. **Documentation**
   - Automatic component documentation from PropTypes/TypeScript
   - Living examples that stay in sync with code
   - Interactive playground for exploring component variants

3. **Design System**
   - Visual catalog of all UI components
   - Ensures consistency across the application
   - Reference for developers and designers

4. **Testing**
   - Visual regression testing with Chromatic
   - Accessibility testing with a11y addon
   - Interaction testing for complex workflows

5. **Collaboration**
   - Share component library with team members
   - Get feedback before integrating into main app
   - Onboard new developers faster

## Recommended Setup

### Installation

```bash
# Using Bun (our package manager)
cd frontend
bun add -D @storybook/react-vite @storybook/addon-essentials @storybook/addon-a11y @storybook/addon-interactions @storybook/test @storybook/blocks
bun add -D storybook
```

### Initialize Storybook

```bash
bunx storybook@latest init --type react --builder vite
```

### Project Structure

```
frontend/
├── .storybook/
│   ├── main.ts              # Storybook configuration
│   ├── preview.ts           # Global decorators and parameters
│   └── manager.ts           # Customize Storybook UI
├── src/
│   ├── components/
│   │   ├── forms/
│   │   │   ├── Form.tsx
│   │   │   ├── Form.stories.tsx        # Form stories
│   │   │   ├── TextField.tsx
│   │   │   ├── TextField.stories.tsx   # TextField stories
│   │   │   └── ...
│   │   └── ...
│   └── stories/
│       ├── Introduction.mdx            # Welcome page
│       ├── design-tokens.stories.tsx   # Design system tokens
│       └── guidelines.mdx              # Usage guidelines
└── package.json
```

## Storybook Configuration

### .storybook/main.ts

```typescript
import type { StorybookConfig } from '@storybook/react-vite';
import { mergeConfig } from 'vite';

const config: StorybookConfig = {
  stories: [
    '../src/**/*.mdx',
    '../src/**/*.stories.@(js|jsx|ts|tsx)',
  ],
  addons: [
    '@storybook/addon-essentials',
    '@storybook/addon-a11y',
    '@storybook/addon-interactions',
  ],
  framework: {
    name: '@storybook/react-vite',
    options: {},
  },
  viteFinal: async (config) => {
    return mergeConfig(config, {
      // Add Vite config customizations here
      resolve: {
        alias: {
          '@': '/src',
        },
      },
    });
  },
};

export default config;
```

### .storybook/preview.ts

```typescript
import type { Preview } from '@storybook/react';
import '../src/index.css'; // Import Tailwind styles

const preview: Preview = {
  parameters: {
    actions: { argTypesRegex: '^on[A-Z].*' },
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/,
      },
    },
    backgrounds: {
      default: 'light',
      values: [
        {
          name: 'light',
          value: '#ffffff',
        },
        {
          name: 'dark',
          value: '#1a202c',
        },
      ],
    },
  },
  globalTypes: {
    theme: {
      name: 'Theme',
      description: 'Global theme for components',
      defaultValue: 'light',
      toolbar: {
        icon: 'circlehollow',
        items: ['light', 'dark'],
        showName: true,
      },
    },
  },
  decorators: [
    (Story, context) => {
      const theme = context.globals.theme;
      return (
        <div className={theme === 'dark' ? 'dark' : ''}>
          <div className="bg-white dark:bg-gray-900 p-4 min-h-screen">
            <Story />
          </div>
        </div>
      );
    },
  ],
};

export default preview;
```

## Writing Stories

### Basic Story Example - TextField

```typescript
// src/components/forms/TextField.stories.tsx
import type { Meta, StoryObj } from '@storybook/react';
import { FormProvider, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from './TextField';

const meta = {
  title: 'Forms/TextField',
  component: TextField,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  decorators: [
    (Story) => {
      const schema = z.object({
        email: z.string().email('Invalid email'),
      });
      const methods = useForm({
        resolver: zodResolver(schema),
        mode: 'onBlur',
      });
      return (
        <FormProvider {...methods}>
          <form className="w-96">
            <Story />
          </form>
        </FormProvider>
      );
    },
  ],
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
    helperText: "We'll never share your email",
    required: true,
  },
};

export const Disabled: Story = {
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
    disabled: true,
  },
};

export const WithError: Story = {
  decorators: [
    (Story) => {
      const schema = z.object({
        email: z.string().email('Invalid email address'),
      });
      const methods = useForm({
        resolver: zodResolver(schema),
        mode: 'onBlur',
        defaultValues: { email: 'invalid-email' },
      });
      // Trigger validation
      methods.trigger('email');
      return (
        <FormProvider {...methods}>
          <form className="w-96">
            <Story />
          </form>
        </FormProvider>
      );
    },
  ],
  args: {
    name: 'email',
    label: 'Email Address',
    placeholder: 'you@example.com',
  },
};
```

### Complex Story Example - Complete Form

```typescript
// src/components/forms/BillForm.stories.tsx
import type { Meta, StoryObj } from '@storybook/react';
import { z } from 'zod';
import { Form, TextField, NumberField, Select } from './index';

const billSchema = z.object({
  name: z.string().min(1, 'Bill name is required'),
  amount: z.number().min(0.01, 'Amount must be greater than 0'),
  frequency: z.string().min(1, 'Please select a frequency'),
});

type BillFormData = z.infer<typeof billSchema>;

const BillFormComponent = () => {
  const handleSubmit = (data: BillFormData) => {
    console.log('Form submitted:', data);
    alert('Bill created! Check console.');
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
      }}
    >
      <TextField name="name" label="Bill Name" placeholder="e.g., Electric Bill" required />
      <NumberField name="amount" label="Amount" placeholder="0.00" required suffix="$" />
      <Select name="frequency" label="Frequency" placeholder="Select frequency" required options={frequencyOptions} />
      <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700">
        Create Bill
      </button>
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
```

## Addons to Use

### Essential Addons

1. **@storybook/addon-essentials**
   - Controls: Interactive controls for component props
   - Actions: Log event handlers
   - Viewport: Test responsive designs
   - Backgrounds: Change background colors
   - Toolbars: Custom toolbar items

2. **@storybook/addon-a11y**
   - Accessibility testing and violations detection
   - ARIA attributes validation
   - Color contrast checking

3. **@storybook/addon-interactions**
   - Test user interactions
   - Automated interaction testing
   - Step-by-step interaction debugging

### Optional Addons

1. **@storybook/addon-links**
   - Link between stories
   - Navigate story hierarchy

2. **@chromatic-com/storybook**
   - Visual regression testing
   - Automated UI review
   - Component versioning

3. **storybook-dark-mode**
   - Toggle dark mode easily
   - Persist dark mode preference

## Stories to Create for Money Matters

### Phase 1: Core Components

1. **Form Components** (Priority: High)
   - Form wrapper
   - TextField (all variants)
   - NumberField (with/without currency)
   - Select
   - TextArea
   - SearchInput

2. **Layout Components**
   - StatCard
   - InfoCard
   - DataTable
   - EmptyState

3. **Feedback Components**
   - AlertBanner
   - Toast/Snackbar
   - Modal/Dialog
   - Badge
   - Chip

### Phase 2: Complex Components

4. **Data Visualization**
   - Line charts
   - Bar charts
   - Donut charts
   - Forecast timeline

5. **Interactive Components**
   - DatePicker
   - Toggle/Switch
   - SegmentedControl
   - Button (all variants)

### Phase 3: Page Templates

6. **Complete Pages**
   - Dashboard layout
   - Bill management
   - Goal tracking
   - Account overview

## NPM Scripts

Add to `frontend/package.json`:

```json
{
  "scripts": {
    "storybook": "storybook dev -p 6006",
    "build-storybook": "storybook build",
    "storybook:test": "test-storybook"
  }
}
```

## Running Storybook

```bash
# Development mode
bun run storybook

# Build static Storybook
bun run build-storybook

# Run interaction tests
bun run storybook:test
```

Storybook will be available at: http://localhost:6006

## Best Practices

### 1. Story Naming Convention

```typescript
// Group stories logically
'Forms/TextField'           // Component category / Component name
'Layout/StatCard'           // Layout components
'Feedback/AlertBanner'      // Feedback components
'Pages/Dashboard'           // Complete pages
```

### 2. Use TypeScript

Always use TypeScript for stories to get type safety and autocompletion.

### 3. Document Props

Use JSDoc comments for component props - they'll appear in Storybook docs:

```typescript
interface TextFieldProps {
  /** The field name (must match form schema) */
  name: string;
  /** Field label displayed above input */
  label?: string;
  /** Placeholder text shown when empty */
  placeholder?: string;
}
```

### 4. Create Multiple States

Show all possible states of a component:
- Default
- With data
- Loading
- Error
- Disabled
- Empty
- Edge cases

### 5. Use Decorators

Use decorators to provide necessary context (forms, providers, etc.)

### 6. Add Interaction Tests

Test user interactions with `@storybook/addon-interactions`:

```typescript
export const SubmitForm: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    await userEvent.type(canvas.getByLabelText('Email'), 'test@example.com');
    await userEvent.click(canvas.getByRole('button', { name: /submit/i }));

    await expect(canvas.getByText('Success!')).toBeInTheDocument();
  },
};
```

## Integration with CI/CD

### GitHub Actions for Chromatic

```yaml
# .github/workflows/chromatic.yml
name: Chromatic

on: push

jobs:
  chromatic:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0
      - uses: oven-sh/setup-bun@v1
      - run: bun install
      - uses: chromaui/action@latest
        with:
          projectToken: ${{ secrets.CHROMATIC_PROJECT_TOKEN }}
          workingDir: frontend
```

## Documentation Structure in Storybook

Create MDX files for documentation:

```mdx
<!-- src/stories/Introduction.mdx -->
import { Meta } from '@storybook/blocks';

<Meta title="Introduction" />

# Money Matters Component Library

Welcome to the Money Matters component library! This Storybook contains all
UI components used in the application.

## Getting Started

Browse the components in the sidebar, organized by category:
- **Forms**: Input components with validation
- **Layout**: Cards, tables, and layout components
- **Feedback**: Alerts, modals, and notifications
- **Data**: Charts and data visualizations

## Design Principles

- **Clean and minimal**: Focus on clarity
- **Accessible**: WCAG 2.1 AA compliant
- **Responsive**: Mobile-first design
- **Dark mode**: Full dark mode support
```

## Deployment

### Build and Deploy

```bash
# Build static Storybook
bun run build-storybook

# Output directory: storybook-static/
# Deploy to: GitHub Pages, Netlify, Vercel, etc.
```

### Chromatic (Recommended)

Chromatic provides:
- Automatic deployment on every commit
- Visual regression testing
- Review workflow for UI changes
- Component versioning

```bash
# Install Chromatic
bun add -D chromatic

# Publish to Chromatic
bunx chromatic --project-token=<your-token>
```

## Timeline and Effort

### Phase 1: Setup (1-2 days)
- Install and configure Storybook
- Set up Tailwind integration
- Create basic decorators and themes
- Write first few component stories

### Phase 2: Core Components (3-5 days)
- Form components stories (all variants)
- Layout components stories
- Feedback components stories

### Phase 3: Advanced Features (2-3 days)
- Interaction tests
- Accessibility tests
- Data visualization components

### Phase 4: Documentation (1-2 days)
- Create MDX documentation pages
- Design system documentation
- Usage guidelines

**Total Estimated Time**: 1-2 weeks for comprehensive setup

## Recommendations

### Should You Implement Storybook?

**Yes, implement it if:**
- ✅ You're building a component library (you are!)
- ✅ You want faster component development
- ✅ You need a design system reference
- ✅ You want to collaborate with designers
- ✅ You plan to scale the application
- ✅ You want automated visual testing

**Maybe wait if:**
- ❌ Very tight deadline (MVP first, Storybook later)
- ❌ Solo developer with no collaboration needs
- ❌ Components are unlikely to be reused

### For Money Matters: **Strongly Recommended**

Given that:
1. You're building a comprehensive component library
2. Multiple form components need thorough testing
3. Design consistency is important for financial app
4. Components will be reused across multiple screens
5. You want to demonstrate component quality

**Storybook is a valuable investment.**

## Next Steps

1. **Approve Storybook implementation**
2. **Install and configure** (use guide above)
3. **Create stories for form components** (already built)
4. **Add accessibility testing**
5. **Document design system**
6. **Set up Chromatic** (optional but recommended)

## Resources

- [Storybook Documentation](https://storybook.js.org/docs)
- [Storybook for React](https://storybook.js.org/docs/react/get-started/install)
- [Chromatic](https://www.chromatic.com/)
- [Storybook Addons](https://storybook.js.org/addons)
- [Component Story Format](https://storybook.js.org/docs/react/api/csf)

🤖 Submitted by George with love ♥
