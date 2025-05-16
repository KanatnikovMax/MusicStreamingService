import * as React from "react";

interface TabsProps {
  defaultValue?: string;
  value?: string;
  onValueChange?: (value: string) => void;
  children: React.ReactNode;
  className?: string;
}

interface TabsListProps {
  children: React.ReactNode;
  className?: string;
}

interface TabsTriggerProps {
  value: string;
  children: React.ReactNode;
  className?: string;
  disabled?: boolean;
}

interface TabsContentProps {
  value: string;
  children: React.ReactNode;
  className?: string;
}

const TabsContext = React.createContext<{
  value: string;
  onValueChange: (value: string) => void;
}>({
  value: "",
  onValueChange: () => {},
});

export const Tabs = ({
  defaultValue,
  value,
  onValueChange,
  children,
  className = "",
}: TabsProps) => {
  const [tabValue, setTabValue] = React.useState(defaultValue || "");

  const contextValue = React.useMemo(() => {
    return {
      value: value !== undefined ? value : tabValue,
      onValueChange: (newValue: string) => {
        if (value === undefined) {
          setTabValue(newValue);
        }
        onValueChange?.(newValue);
      },
    };
  }, [value, tabValue, onValueChange]);

  return (
    <TabsContext.Provider value={contextValue}>
      <div className={className}>{children}</div>
    </TabsContext.Provider>
  );
};

export const TabsList = ({ children, className = "" }: TabsListProps) => {
  return (
    <div className={`flex space-x-1 ${className}`} role="tablist">
      {children}
    </div>
  );
};

export const TabsTrigger = ({
  value,
  children,
  className = "",
  disabled = false,
}: TabsTriggerProps) => {
  const { value: selectedValue, onValueChange } = React.useContext(TabsContext);
  const isSelected = selectedValue === value;

  return (
    <button
      role="tab"
      aria-selected={isSelected}
      disabled={disabled}
      className={`px-3 py-1.5 text-sm font-medium transition-all outline-none ${
        isSelected
          ? "bg-white text-gray-900 shadow-sm"
          : "text-gray-500 hover:text-gray-900"
      } ${disabled ? "opacity-50 cursor-not-allowed" : ""} ${className}`}
      onClick={() => onValueChange(value)}
    >
      {children}
    </button>
  );
};

export const TabsContent = ({
  value,
  children,
  className = "",
}: TabsContentProps) => {
  const { value: selectedValue } = React.useContext(TabsContext);
  const isSelected = selectedValue === value;

  if (!isSelected) return null;

  return (
    <div role="tabpanel" className={className}>
      {children}
    </div>
  );
};