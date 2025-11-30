import type { Meta, StoryObj } from '@storybook/react';
import { useState } from 'react';
import { SearchInput } from './SearchInput';

// Wrapper for controlled component
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const SearchInputWrapper = (args: any) => {
  const [value, setValue] = useState(args.value || '');

  return (
    <div className="w-96">
      <SearchInput {...args} value={value} onChange={setValue} />
      {value && (
        <p className="mt-3 text-sm text-gray-600 dark:text-gray-400">
          Search query: <strong>{value}</strong>
        </p>
      )}
    </div>
  );
};

const meta = {
  title: 'Forms/SearchInput',
  component: SearchInput,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  render: (args) => <SearchInputWrapper {...args} />,
} satisfies Meta<typeof SearchInput>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    placeholder: 'Search...',
  },
};

export const WithCustomPlaceholder: Story = {
  args: {
    placeholder: 'Search transactions...',
  },
};

export const WithInitialValue: Story = {
  args: {
    value: 'Initial search query',
    placeholder: 'Search...',
  },
};

export const WithoutClearButton: Story = {
  args: {
    placeholder: 'Search...',
    showClearButton: false,
  },
};
