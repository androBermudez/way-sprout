import { useQuery, useQueryClient } from "@tanstack/react-query"
import { ArrowLeft } from "lucide-react"
import { useState } from "react"
import { Link, useParams } from "react-router-dom"

import {
  getJobApplicationById,
  updateJobApplication,
  type ApplicationStatusValue,
} from "@/api/jobApplication"
import { ApplicationForm, type ApplicationFormValues } from "@/components/ApplicationForm"
import { Badge } from "@/components/ui/badge"
import { Button, buttonVariants } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

export function ApplicationDetailsPage() {
  const { id } = useParams<{ id: string }>()
  const queryClient = useQueryClient()
  const [isEditing, setIsEditing] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const { data, isLoading, isError } = useQuery({
    queryKey: ["job-application", id],
    queryFn: () => getJobApplicationById(id!),
    enabled: !!id,
  })

  const handleSave = async (values: ApplicationFormValues) => {
    setIsSubmitting(true)
    try {
      await updateJobApplication(id!, {
        status: values.status,
        description: values.description,
        url: values.url || undefined,
      })
      await queryClient.invalidateQueries({ queryKey: ["job-application", id] })
      setIsEditing(false)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <>
      <div className="py-6">
        <Link to="/applications" className={buttonVariants({ variant: "outline", size: "sm" })}>
          <ArrowLeft />
          Back to applications
        </Link>
      </div>

      {isLoading && <p>Loading...</p>}

      {isError && <p className="text-destructive">Failed to load application.</p>}

      {!isLoading && !isError && !data && <p>Application not found.</p>}

      {data && !isEditing && (
        <Card className="max-w-2xl">
          <CardHeader>
            <div className="flex items-start justify-between">
              <div>
                <CardTitle>{data.position}</CardTitle>
                <CardDescription>{data.company}</CardDescription>
              </div>
              <Button variant="outline" size="sm" onClick={() => setIsEditing(true)}>
                Edit
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-2">
              <Badge>{data.status}</Badge>
              <span className="text-sm text-muted-foreground">Applied on {data.appliedOn}</span>
            </div>
            <p className="whitespace-pre-line">{data.description ?? "No description available."}</p>
            {data.url && (
              <a
                href={data.url}
                target="_blank"
                rel="noopener noreferrer"
                className="text-sm text-blue-600 underline"
              >
                {data.url}
              </a>
            )}
          </CardContent>
        </Card>
      )}

      {data && isEditing && (
        <ApplicationForm
          showStatusField
          initialValues={{
            company: data.company,
            position: data.position,
            appliedOn: data.appliedOn,
            description: data.description ?? "",
            url: data.url ?? "",
            status: data.status as ApplicationStatusValue,
          }}
          onSubmit={handleSave}
          onCancel={() => setIsEditing(false)}
          isSubmitting={isSubmitting}
        />
      )}
    </>
  )
}
