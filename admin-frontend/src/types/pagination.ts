export interface PaginationRequest<TCursor> {
    cursor?: TCursor;
    pageSize: number;
}

export interface PaginatedResponse<TCursor, TItems> {
    cursor?: TCursor;
    items: TItems[];
}