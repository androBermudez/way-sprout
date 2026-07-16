import { useState } from "react"
import { useNavigate } from "react-router-dom"

import { createJobApplication } from "@/api/jobApplication"
import { ApplicationForm, type ApplicationFormValues } from "@/components/ApplicationForm"

export function ApplicationFormPage() {
  const navigate = useNavigate()
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (values: ApplicationFormValues) => {
    setIsSubmitting(true)
    try {
      const id = await createJobApplication({
        company: values.company,
        position: values.position,
        description: values.description,
        appliedOn: values.appliedOn,
        url: values.url || undefined,
      })
      navigate(`/applications/${id}`)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <ApplicationForm
      onSubmit={handleSubmit}
      onCancel={() => navigate("/applications")}
      isSubmitting={isSubmitting}
    />
  )
}
