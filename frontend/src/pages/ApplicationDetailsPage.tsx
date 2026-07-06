import { Link, useParams } from "react-router-dom"
import { useQuery } from "@tanstack/react-query"
import { ArrowLeft } from "lucide-react"

import { getJobApplicationById } from "@/api/jobApplication"
import { Badge } from "@/components/ui/badge"
import { buttonVariants } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

export function ApplicationDetailsPage() {
  const { id } = useParams<{ id: string }>()

  const { data, isLoading, isError } = useQuery({
    queryKey: ["job-application", id],
    queryFn: () => getJobApplicationById(id!),
    enabled: !!id,
  })

  return (
    <>
      <div className="py-6">
        <Link
          to="/applications"
          className={buttonVariants({ variant: "outline", size: "sm" })}
        >
          <ArrowLeft />
          Back to applications
        </Link>
      </div>

      {isLoading && <p>Loading...</p>}

      {isError && (
        <p className="text-destructive">Failed to load application.</p>
      )}

      {!isLoading && !isError && !data && <p>Application not found.</p>}

      {data && (
        <Card className="max-w-2xl">
          <CardHeader>
            <CardTitle>{data.position}</CardTitle>
            <CardDescription>{data.company}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-2">
              <Badge>{data.status}</Badge>
              <span className="text-sm text-muted-foreground">
                Applied on {data.appliedOn}
              </span>
            </div>
            <p className="whitespace-pre-line">
              {data.description ?? "No description available."}
            </p>
          </CardContent>
        </Card>
      )}
    </>
  )
}
