export interface PaginationRequest<TCursor> {
    cursor?: TCursor;
    pageSize: number;
}

export interface PaginatedResponse<TCursor, TItems> {
    cursor?: TCursor;
    items: TItems[];
}

export interface PopularityCursor {
    cursorPlayCount: number;
    cursorCreatedAt: string;
}

export interface PopularityPaginationRequest {
    cursorPlayCount?: number;
    cursorCreatedAt?: string;
    pageSize: number;
}