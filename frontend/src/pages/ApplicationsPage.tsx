import { useMemo, useState } from "react"
import { useNavigate } from "react-router-dom"
import { useQuery } from "@tanstack/react-query"
import { ArrowDown, ArrowUp, ChevronsUpDown } from "lucide-react"

import {
  APPLICATION_STATUSES,
  DATE_RANGE_PRESETS,
  getJobApplications,
  type ApplicationStatusValue,
  type DateRangePresetValue,
  type SortCriteriaValue,
  type SortDirectionValue,
} from "@/api/jobApplication"
import { useDebouncedValue } from "@/lib/useDebouncedValue"
import { Badge } from "@/components/ui/badge"
import { buttonVariants } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { Input } from "@/components/ui/input"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

const DATE_RANGE_LABELS: Record<DateRangePresetValue, string> = {
  Today: "Today",
  Last2Days: "Last 2 days",
  ThisWeek: "This week",
  Last7Days: "Last 7 days",
  ThisMonth: "This month",
  Last30Days: "Last 30 days",
  Last90Days: "Last 90 days",
}

interface SortableHeaderProps {
  label: string
  column: SortCriteriaValue
  sortBy: SortCriteriaValue | undefined
  direction: SortDirectionValue | undefined
  onSort: (column: SortCriteriaValue) => void
}

function SortableHeader({
  label,
  column,
  sortBy,
  direction,
  onSort,
}: SortableHeaderProps) {
  const isActive = sortBy === column
  const Icon = isActive
    ? direction === "Asc"
      ? ArrowUp
      : ArrowDown
    : ChevronsUpDown

  return (
    <TableHead>
      <button
        type="button"
        className="inline-flex items-center gap-1 font-medium"
        onClick={() => onSort(column)}
      >
        {label}
        <Icon className="size-3.5 text-muted-foreground" />
      </button>
    </TableHead>
  )
}

export function ApplicationsPage() {
  const navigate = useNavigate()

  const [searchText, setSearchText] = useState("")
  const debouncedSearchText = useDebouncedValue(searchText, 300)

  const [selectedStatuses, setSelectedStatuses] = useState<
    ApplicationStatusValue[]
  >([])
  const [appliedRange, setAppliedRange] = useState<
    DateRangePresetValue | undefined
  >(undefined)
  const [sortBy, setSortBy] = useState<SortCriteriaValue | undefined>(
    undefined,
  )
  const [direction, setDirection] = useState<SortDirectionValue | undefined>(
    undefined,
  )

  const query = useMemo(
    () => ({
      searchText: debouncedSearchText || undefined,
      statuses: selectedStatuses,
      appliedRange,
      sortBy,
      direction,
    }),
    [debouncedSearchText, selectedStatuses, appliedRange, sortBy, direction],
  )

  const { data, isLoading, isError } = useQuery({
    queryKey: ["job-applications", query],
    queryFn: () => getJobApplications(query),
  })

  function toggleStatus(status: ApplicationStatusValue, checked: boolean) {
    setSelectedStatuses((current) =>
      checked ? [...current, status] : current.filter((s) => s !== status),
    )
  }

  function handleSort(column: SortCriteriaValue) {
    if (sortBy !== column) {
      setSortBy(column)
      setDirection("Asc")
    } else if (direction === "Asc") {
      setDirection("Desc")
    } else {
      setSortBy(undefined)
      setDirection(undefined)
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2">
        <Input
          placeholder="Quick filter..."
          value={searchText}
          onChange={(event) => setSearchText(event.target.value)}
          className="max-w-xs"
        />

        <Popover>
          <PopoverTrigger
            className={buttonVariants({ variant: "outline", size: "sm" })}
          >
            Status
            {selectedStatuses.length > 0 && ` (${selectedStatuses.length})`}
          </PopoverTrigger>
          <PopoverContent className="w-48 space-y-2 p-3">
            {APPLICATION_STATUSES.map((status) => (
              <label key={status} className="flex items-center gap-2 text-sm">
                <Checkbox
                  checked={selectedStatuses.includes(status)}
                  onCheckedChange={(checked) => toggleStatus(status, checked)}
                />
                {status}
              </label>
            ))}
          </PopoverContent>
        </Popover>

        <Select
          value={appliedRange ?? "All"}
          onValueChange={(value) =>
            setAppliedRange(
              value === "All" ? undefined : (value as DateRangePresetValue),
            )
          }
        >
          <SelectTrigger size="sm">
            <SelectValue placeholder="Applied" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="All">All</SelectItem>
            {DATE_RANGE_PRESETS.map((preset) => (
              <SelectItem key={preset} value={preset}>
                {DATE_RANGE_LABELS[preset]}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {isLoading && <p>Loading...</p>}
      {isError && <p className="text-destructive">Failed to load applications.</p>}

      {data && (
        <Table>
          <TableHeader>
            <TableRow>
              <SortableHeader
                label="Company"
                column="Company"
                sortBy={sortBy}
                direction={direction}
                onSort={handleSort}
              />
              <SortableHeader
                label="Position"
                column="Position"
                sortBy={sortBy}
                direction={direction}
                onSort={handleSort}
              />
              <TableHead>Status</TableHead>
              <SortableHeader
                label="Applied on"
                column="DateApplied"
                sortBy={sortBy}
                direction={direction}
                onSort={handleSort}
              />
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.map((application) => (
              <TableRow
                key={application.id}
                className="cursor-pointer"
                onClick={() => navigate(`/applications/${application.id}`)}
              >
                <TableCell>{application.company}</TableCell>
                <TableCell>{application.position}</TableCell>
                <TableCell>
                  <Badge>{application.status}</Badge>
                </TableCell>
                <TableCell>{application.appliedOn}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </div>
  )
}
