'use server'
import { auth } from '@/auth';
import { Auction, Bid, PagedResult } from '@/types';
import { fetchWrapper } from '../lib/fetchWrapper';
import { FieldValues } from 'react-hook-form';

export async function getData(query: string): Promise<PagedResult<Auction>> {
  return await fetchWrapper.get(`search${query}`)
}

export async function updateAuctionTest() {
  const data = {
    mileage: Math.floor(Math.random() * 10000) + 1
  }

  return await fetchWrapper.put('auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', data);
}

export const createAuction = async (data: FieldValues) => {
  return await fetchWrapper.post('auctions', data);
}

export async function getDetailedViewData(id: string): Promise<Auction> {
  return fetchWrapper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string) {
  return fetchWrapper.put(`auctions/${id}`, data);
}

export async function deleteAuction(id: string) {
  return fetchWrapper.del(`auctions/${id}`);
}

export async function getBidsForAuction(auctionId: string): Promise<Bid[]> {
  return fetchWrapper.get(`bids/${auctionId}`);
}

export async function placeBidForAuction(auctionId: string, amount: number) {
  return fetchWrapper.post(`bids?auctionId=${auctionId}&amount=${amount}`, {});
}