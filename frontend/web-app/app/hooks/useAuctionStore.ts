import { Auction, PagedResult } from "@/types"
import { create } from "zustand";

type State = {
    auctions: Auction[];
    totalCount: number;
    pageCount: number;
}

type Actions = {
    setData: (date: PagedResult<Auction>) => void;
    setCurrentPrice: (auctionId: string, newPrice: number) => void;
}

const initialState: State = {
    auctions: [],
    totalCount: 0,
    pageCount: 0
}

export const useAuctionStore = create<State & Actions>((set) => ({
    ...initialState,
    setData: (data: PagedResult<Auction>) => set(() => ({
        auctions: data.results,
        totalCount: data.totalCount,
        pageCount: data.pageCount
    })),
    setCurrentPrice: (auctionId: string, newPrice: number) => set((state) => ({
        auctions: state.auctions.map(auction =>
            auction.id === auctionId ? { ...auction, currentHighBid: newPrice } : auction
        )
    }))
}))