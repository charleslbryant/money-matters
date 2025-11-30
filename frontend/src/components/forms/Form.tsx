import React from 'react';
import {
  useForm,
  UseFormReturn,
  FieldValues,
  SubmitHandler,
  FormProvider,
} from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ZodSchema } from 'zod';

interface FormProps<T extends FieldValues> {
  children: React.ReactNode;
  onSubmit: SubmitHandler<T>;
  schema: ZodSchema<T>;
  defaultValues?: Partial<T>;
  className?: string;
  id?: string;
}

export function Form<T extends FieldValues>({
  children,
  onSubmit,
  schema,
  defaultValues,
  className = '',
  id,
}: FormProps<T>) {
  const methods = useForm<T>({
    resolver: zodResolver(schema),
    defaultValues: defaultValues as Partial<T>,
    mode: 'onBlur',
  });

  return (
    <FormProvider {...methods}>
      <form
        id={id}
        onSubmit={methods.handleSubmit(onSubmit)}
        className={className}
        noValidate
      >
        {children}
      </form>
    </FormProvider>
  );
}

interface FormFieldProps {
  children: React.ReactNode;
  className?: string;
}

export function FormField({ children, className = '' }: FormFieldProps) {
  return <div className={`mb-4 ${className}`}>{children}</div>;
}

interface FormErrorProps {
  message?: string;
  className?: string;
}

export function FormError({ message, className = '' }: FormErrorProps) {
  if (!message) return null;

  return (
    <p
      className={`mt-1 text-sm text-red-600 dark:text-red-400 ${className}`}
      role="alert"
    >
      {message}
    </p>
  );
}

interface FormLabelProps {
  htmlFor: string;
  children: React.ReactNode;
  required?: boolean;
  className?: string;
}

export function FormLabel({
  htmlFor,
  children,
  required = false,
  className = '',
}: FormLabelProps) {
  return (
    <label
      htmlFor={htmlFor}
      className={`block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1 ${className}`}
    >
      {children}
      {required && <span className="text-red-500 ml-1">*</span>}
    </label>
  );
}

interface FormHelperTextProps {
  children: React.ReactNode;
  className?: string;
}

export function FormHelperText({
  children,
  className = '',
}: FormHelperTextProps) {
  return (
    <p className={`mt-1 text-sm text-gray-500 dark:text-gray-400 ${className}`}>
      {children}
    </p>
  );
}

export type { UseFormReturn };
