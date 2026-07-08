export const APPLICATION_STATUSES = [
  "Applied",
  "Interviewing",
  "Offer",
  "Rejected",
  "Withdrawn",
] as const

export type ApplicationStatusValue = (typeof APPLICATION_STATUSES)[number]

export const DATE_RANGE_PRESETS = [
  "Today",
  "Last2Days",
  "ThisWeek",
  "Last7Days",
  "ThisMonth",
  "Last30Days",
  "Last90Days",
] as const

export type DateRangePresetValue = (typeof DATE_RANGE_PRESETS)[number]

export const SORT_CRITERIA = ["Company", "Position", "DateApplied"] as const

export type SortCriteriaValue = (typeof SORT_CRITERIA)[number]

export type SortDirectionValue = "Asc" | "Desc"

export interface JobApplication {
  id: string
  company: string
  position: string
  status: string
  appliedOn: string
  description?: string
}

export interface JobApplicationsQuery {
  searchText?: string
  statuses?: ApplicationStatusValue[]
  appliedRange?: DateRangePresetValue
  sortBy?: SortCriteriaValue
  direction?: SortDirectionValue
}

const API_BASE_URL = "/api/v1"

export async function getJobApplications(
  query: JobApplicationsQuery = {},
): Promise<JobApplication[]> {
  const params = new URLSearchParams()

  if (query.searchText) {
    params.set("q", query.searchText)
  }

  for (const status of query.statuses ?? []) {
    params.append("status", status)
  }

  if (query.appliedRange) {
    params.set("applied-range", query.appliedRange)
  }

  if (query.sortBy) {
    params.set("sort-by", query.sortBy)
  }

  if (query.direction) {
    params.set("direction", query.direction)
  }

  const queryString = params.toString()
  const response = await fetch(
    `${API_BASE_URL}/applications${queryString ? `?${queryString}` : ""}`,
  )

  if (!response.ok) {
    throw new Error(`Failed to load applications: ${response.status}`)
  }

  return response.json()
}

export async function getJobApplicationById(id: string): Promise<JobApplication | null> {
  const response = await fetch(`${API_BASE_URL}/applications/${id}`)
  const status = response.status

  if (status === 404) {
    return null
  }

  if (!response.ok) {
    throw new Error(`Failed to load application with id ${id}: ${status}`)
  }

  return response.json()
}
