import React from 'react';
import { useFormContext, Controller } from 'react-hook-form';
import { FormField, FormLabel, FormError, FormHelperText } from './Form';

interface NumberFieldProps {
  name: string;
  label?: string;
  placeholder?: string;
  helperText?: string;
  required?: boolean;
  disabled?: boolean;
  min?: number;
  max?: number;
  step?: number;
  prefix?: string;
  suffix?: string;
  className?: string;
  inputClassName?: string;
}

export function NumberField({
  name,
  label,
  placeholder,
  helperText,
  required = false,
  disabled = false,
  min,
  max,
  step = 0.01,
  prefix,
  suffix = '$',
  className = '',
  inputClassName = '',
}: NumberFieldProps) {
  const {
    control,
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

  const wrapperClasses = prefix || suffix ? 'relative' : '';
  const paddingLeft = prefix ? 'pl-7' : '';
  const paddingRight = suffix ? 'pr-7' : '';

  return (
    <FormField className={className}>
      {label && (
        <FormLabel htmlFor={name} required={required}>
          {label}
        </FormLabel>
      )}
      <div className={wrapperClasses}>
        {prefix && (
          <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-sm text-gray-500 dark:text-gray-400 pointer-events-none">
            {prefix}
          </span>
        )}
        <Controller
          name={name}
          control={control}
          render={({ field }) => (
            <input
              {...field}
              id={name}
              type="number"
              step={step}
              min={min}
              max={max}
              aria-invalid={hasError}
              aria-describedby={
                hasError
                  ? `${name}-error`
                  : helperText
                    ? `${name}-helper`
                    : undefined
              }
              placeholder={placeholder}
              disabled={disabled}
              className={`${inputClasses} ${paddingLeft} ${paddingRight}`}
              onChange={(e) => {
                const value = e.target.value;
                field.onChange(value === '' ? '' : parseFloat(value));
              }}
              value={field.value ?? ''}
            />
          )}
        />
        {suffix && (
          <span className="absolute inset-y-0 right-0 flex items-center pr-3 text-sm text-gray-500 dark:text-gray-400 pointer-events-none">
            {suffix}
          </span>
        )}
      </div>
      {hasError ? (
        <FormError message={error} />
      ) : helperText ? (
        <FormHelperText>{helperText}</FormHelperText>
      ) : null}
    </FormField>
  );
}
