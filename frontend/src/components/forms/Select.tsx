import React from 'react';
import { useFormContext } from 'react-hook-form';
import { FormField, FormLabel, FormError, FormHelperText } from './Form';

interface SelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

interface SelectProps {
  name: string;
  label?: string;
  placeholder?: string;
  helperText?: string;
  required?: boolean;
  disabled?: boolean;
  options: SelectOption[];
  className?: string;
  selectClassName?: string;
}

export function Select({
  name,
  label,
  placeholder,
  helperText,
  required = false,
  disabled = false,
  options,
  className = '',
  selectClassName = '',
}: SelectProps) {
  const {
    register,
    formState: { errors },
  } = useFormContext();

  const error = errors[name]?.message as string | undefined;
  const hasError = !!error;

  const baseSelectClasses =
    'block w-full rounded-md border px-3 py-2 pr-10 text-sm shadow-sm transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-50 appearance-none bg-no-repeat bg-right';
  const normalClasses =
    'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:ring-blue-500';
  const errorClasses =
    'border-red-500 dark:border-red-400 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-red-500 focus:ring-red-500';

  const selectClasses = `${baseSelectClasses} ${hasError ? errorClasses : normalClasses} ${selectClassName}`;

  return (
    <FormField className={className}>
      {label && (
        <FormLabel htmlFor={name} required={required}>
          {label}
        </FormLabel>
      )}
      <div className="relative">
        <select
          id={name}
          aria-invalid={hasError}
          aria-describedby={
            hasError
              ? `${name}-error`
              : helperText
                ? `${name}-helper`
                : undefined
          }
          disabled={disabled}
          className={selectClasses}
          {...register(name)}
        >
          {placeholder && (
            <option value="" disabled>
              {placeholder}
            </option>
          )}
          {options.map((option) => (
            <option
              key={option.value}
              value={option.value}
              disabled={option.disabled}
            >
              {option.label}
            </option>
          ))}
        </select>
        <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-500 dark:text-gray-400">
          <svg
            className="h-5 w-5"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            aria-hidden="true"
          >
            <path
              fillRule="evenodd"
              d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z"
              clipRule="evenodd"
            />
          </svg>
        </div>
      </div>
      {hasError ? (
        <FormError message={error} />
      ) : helperText ? (
        <FormHelperText>{helperText}</FormHelperText>
      ) : null}
    </FormField>
  );
}
