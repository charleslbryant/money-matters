import React from 'react';
import { useFormContext } from 'react-hook-form';
import { FormField, FormLabel, FormError, FormHelperText } from './Form';

interface TextFieldProps {
  name: string;
  label?: string;
  placeholder?: string;
  helperText?: string;
  required?: boolean;
  disabled?: boolean;
  type?: 'text' | 'email' | 'password' | 'tel' | 'url';
  autoComplete?: string;
  className?: string;
  inputClassName?: string;
}

export function TextField({
  name,
  label,
  placeholder,
  helperText,
  required = false,
  disabled = false,
  type = 'text',
  autoComplete,
  className = '',
  inputClassName = '',
}: TextFieldProps) {
  const {
    register,
    formState: { errors },
  } = useFormContext();

  const error = errors[name]?.message as string | undefined;
  const hasError = !!error;

  const baseInputClasses =
    'block w-full rounded-md border px-3 py-2 text-sm shadow-sm transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-50';
  const normalClasses =
    'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:ring-blue-500';
  const errorClasses =
    'border-red-500 dark:border-red-400 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-red-500 focus:ring-red-500';

  const inputClasses = `${baseInputClasses} ${hasError ? errorClasses : normalClasses} ${inputClassName}`;

  return (
    <FormField className={className}>
      {label && (
        <FormLabel htmlFor={name} required={required}>
          {label}
        </FormLabel>
      )}
      <input
        id={name}
        type={type}
        aria-invalid={hasError}
        aria-describedby={
          hasError ? `${name}-error` : helperText ? `${name}-helper` : undefined
        }
        placeholder={placeholder}
        autoComplete={autoComplete}
        disabled={disabled}
        className={inputClasses}
        {...register(name)}
      />
      {hasError ? (
        <FormError message={error} />
      ) : helperText ? (
        <FormHelperText>{helperText}</FormHelperText>
      ) : null}
    </FormField>
  );
}
