import { useState } from "react"

import { APPLICATION_STATUSES, type ApplicationStatusValue } from "@/api/jobApplication"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"

export interface ApplicationFormValues {
  company: string
  position: string
  description: string
  appliedOn: string
  url: string
  status: ApplicationStatusValue
}

interface ApplicationFormProps {
  initialValues?: Partial<ApplicationFormValues>
  showStatusField?: boolean
  onSubmit: (values: ApplicationFormValues) => Promise<void>
  onCancel: () => void
  isSubmitting: boolean
}

const DEFAULT_VALUES: ApplicationFormValues = {
  company: "",
  position: "",
  description: "",
  appliedOn: "",
  url: "",
  status: "Applied",
}

export function ApplicationForm({
  initialValues,
  showStatusField = false,
  onSubmit,
  onCancel,
  isSubmitting,
}: ApplicationFormProps) {
  const [values, setValues] = useState<ApplicationFormValues>({
    ...DEFAULT_VALUES,
    ...initialValues,
  })
  const [errors, setErrors] = useState<Partial<Record<keyof ApplicationFormValues, string>>>({})
  const [submitError, setSubmitError] = useState<string | null>(null)

  const set = (field: keyof ApplicationFormValues) => (value: string) =>
    setValues((prev) => ({ ...prev, [field]: value }))

  const validate = (): boolean => {
    const next: typeof errors = {}
    if (!values.company.trim()) next.company = "Company is required."
    if (!values.position.trim()) next.position = "Position is required."
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!validate()) return
    setSubmitError(null)
    try {
      await onSubmit(values)
    } catch {
      setSubmitError("Something went wrong. Please try again.")
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-w-lg">
      <div className="space-y-1">
        <label className="text-sm font-medium">Company *</label>
        <Input
          value={values.company}
          onChange={(e) => set("company")(e.target.value)}
          placeholder="Company name"
        />
        {errors.company && <p className="text-sm text-destructive">{errors.company}</p>}
      </div>

      <div className="space-y-1">
        <label className="text-sm font-medium">Position *</label>
        <Input
          value={values.position}
          onChange={(e) => set("position")(e.target.value)}
          placeholder="Job title"
        />
        {errors.position && <p className="text-sm text-destructive">{errors.position}</p>}
      </div>

      <div className="space-y-1">
        <label className="text-sm font-medium">Description</label>
        <Textarea
          value={values.description}
          onChange={(e) => set("description")(e.target.value)}
          placeholder="Role description"
          rows={4}
        />
      </div>

      <div className="space-y-1">
        <label className="text-sm font-medium">Applied on</label>
        <Input
          type="date"
          value={values.appliedOn}
          onChange={(e) => set("appliedOn")(e.target.value)}
        />
      </div>

      <div className="space-y-1">
        <label className="text-sm font-medium">Listing URL</label>
        <Input
          value={values.url}
          onChange={(e) => set("url")(e.target.value)}
          placeholder="https://linkedin.com/jobs/..."
        />
      </div>

      {showStatusField && (
        <div className="space-y-1">
          <label className="text-sm font-medium">Status</label>
          <Select value={values.status} onValueChange={(v) => v && set("status")(v)}>
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {APPLICATION_STATUSES.map((s) => (
                <SelectItem key={s} value={s}>
                  {s}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      )}

      {submitError && <p className="text-sm text-destructive">{submitError}</p>}

      <div className="flex gap-2">
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Saving..." : "Save"}
        </Button>
        <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
      </div>
    </form>
  )
}
