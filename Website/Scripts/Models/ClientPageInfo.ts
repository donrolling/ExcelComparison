/// <reference path="./ClientSearchFilter.ts" />

class ClientPageInfo {
	public Filters: ClientSearchFilter[];
	public OrderBy: string;
	public PageSize: number;
	public PageStart: number;
	public ReadActive: boolean;
	public ReadInactive: boolean;
}