import React from 'react';
import { useFormContext } from 'react-hook-form';
import { FormField, FormLabel, FormError, FormHelperText } from './Form';

interface TextAreaProps {
  name: string;
  label?: string;
  placeholder?: string;
  helperText?: string;
  required?: boolean;
  disabled?: boolean;
  rows?: number;
  maxLength?: number;
  className?: string;
  textAreaClassName?: string;
}

export function TextArea({
  name,
  label,
  placeholder,
  helperText,
  required = false,
  disabled = false,
  rows = 4,
  maxLength,
  className = '',
  textAreaClassName = '',
}: TextAreaProps) {
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext();

  const error = errors[name]?.message as string | undefined;
  const hasError = !!error;
  const currentValue = watch(name) || '';
  const showCharCount = maxLength !== undefined;

  const baseTextAreaClasses =
    'block w-full rounded-md border px-3 py-2 text-sm shadow-sm transition-colors focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:opacity-50 resize-y';
  const normalClasses =
    'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:ring-blue-500';
  const errorClasses =
    'border-red-500 dark:border-red-400 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 focus:border-red-500 focus:ring-red-500';

  const textAreaClasses = `${baseTextAreaClasses} ${hasError ? errorClasses : normalClasses} ${textAreaClassName}`;

  return (
    <FormField className={className}>
      {label && (
        <FormLabel htmlFor={name} required={required}>
          {label}
        </FormLabel>
      )}
      <textarea
        id={name}
        rows={rows}
        maxLength={maxLength}
        aria-invalid={hasError}
        aria-describedby={
          hasError ? `${name}-error` : helperText ? `${name}-helper` : undefined
        }
        placeholder={placeholder}
        disabled={disabled}
        className={textAreaClasses}
        {...register(name)}
      />
      {showCharCount && (
        <div className="mt-1 text-xs text-gray-500 dark:text-gray-400 text-right">
          {currentValue.length}/{maxLength}
        </div>
      )}
      {hasError ? (
        <FormError message={error} />
      ) : helperText ? (
        <FormHelperText>{helperText}</FormHelperText>
      ) : null}
    </FormField>
  );
}
